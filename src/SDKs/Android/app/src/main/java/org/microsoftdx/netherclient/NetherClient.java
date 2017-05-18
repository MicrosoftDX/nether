/* Copyright (c) 2017 Microsoft Corporation. This software is licensed under the MIT License.
 * See the license file delivered with this project for further information.
 */
package org.microsoftdx.netherclient;

import android.util.Log;
import okhttp3.Response;
import org.json.JSONException;
import org.json.JSONObject;
import org.microsoftdx.netherclient.authentication.NetherOAuthManager;
import org.microsoftdx.netherclient.networking.HttpOperationAsyncTask;
import org.microsoftdx.netherclient.networking.ReadResponseAsyncTask;

/**
 * The main interface of the Nether client.
 */
public class NetherClient
        implements NetherOAuthManager.Listener,
                   HttpOperationAsyncTask.Listener,
                   ReadResponseAsyncTask.Listener {
    /**
     * The listener for API call results.
     */
    public interface Listener {
        void onNetherApiCallResult(NetherApiCallResponse netherApiCallResponse);
        void onNetherApiCallFailed(String errorMessage);
        void onAuthenticated(String accessToken);
        void onAuthenticationFailed(String errorMessage);
    }

    /**
     * User roles.
     */
    public enum UserRole {
        ADMIN,
        PLAYER
    }

    /*
     * Constants
     */
    private static final String TAG = NetherClient.class.getSimpleName();

    private static final String URI_API = "api";
    private static final String DEFAULT_NETHER_API_VERSION = "2016-09-01";

    // Identity: Users API/User Logins API
    private static final String URI_IDENTITY_USERS = "identity/users";
    private static final String URI_LOGINS = "logins";
    private static final String URI_LOGINS_PASSWORD = URI_LOGINS + "/password";
    private static final String ID_ACTIVE = "active";
    private static final String ID_PASSWORD = "password";
    private static final String ID_ROLE = "role";
    private static final String USER_ROLE_ADMIN = "Admin";
    private static final String USER_ROLE_PLAYER = "Player";

    // Leaderboards/scores
    private static final String URI_LEADERBOARDS = "leaderboards";
    private static final String URI_SCORES = "scores";
    private static final String ID_COUNTRY = "country";
    private static final String ID_SCORE = "score";

    /*
     * Members
     */
    private Listener mListener;
    private NetherOAuthManager mNetherOAuthManager;
    private HttpOperationAsyncTask mHttpOperationAsyncTask;
    private NetherApiCallResponse mLatestNetherApiCallResponse;
    private String mNetherBaseUrl;
    private String mNetherApiBaseUri;
    private String mNetherClientId;
    private String mNetherClientSecret;
    private String mNetherApiVersion;


    /**
     * Constructor.
     *
     * @param listener The listener.
     * @param netherBaseUrl The base URL of the Nether instance.
     * @param netherClientId The Nether client ID.
     * @param netherClientSecret The Nether client secret.
     */
    public NetherClient(Listener listener, String netherBaseUrl, String netherClientId, String netherClientSecret) {
        mListener = listener;
        mNetherOAuthManager = new NetherOAuthManager(this, netherBaseUrl);
        mNetherBaseUrl = netherBaseUrl;
        mNetherApiBaseUri = mNetherBaseUrl + "/" + URI_API;
        mNetherClientId = netherClientId;
        mNetherClientSecret = netherClientSecret;
        mNetherApiVersion = DEFAULT_NETHER_API_VERSION;
    }

    /**
     * Constructor.
     *
     * @param listener The listener.
     * @param netherBaseUrl The base URL of the Nether instance.
     * @param netherClientId The Nether client ID.
     * @param netherClientSecret The Nether client secret.
     * @param netherApiVersion The Nether API version.
     */
    public NetherClient(Listener listener,
                        String netherBaseUrl,
                        String netherClientId,
                        String netherClientSecret,
                        String netherApiVersion) {
        this(listener, netherBaseUrl, netherClientId, netherClientSecret);
        mNetherApiVersion = netherApiVersion;
    }

    /**
     * @param listener The listener.
     */
    public void setListener(Listener listener) {
        Log.d(TAG, "Setting new listener: " + listener);
        mListener = listener;
    }

    /**
     * @return True, if a user/player has logged in (authenticated). False otherwise.
     */
    public boolean getIsLoggedIn() {
        String accessToken = NetherOAuthManager.getAccessToken();
        return (accessToken != null && accessToken.length() > 0);
    }

    /**
     * Logs in/authenticates the given user.
     *
     * @param userId The ID of the user to login.
     * @param password The password.
     */
    public void login(String userId, String password) {
        mNetherOAuthManager.initiateAuthentication(mNetherClientId, mNetherClientSecret, userId, password);
    }

    /**
     * Retrieves the leaderboard with the given name.
     *
     * @param name The name of the leaderboard.
     */
    public void getLeaderboard(String name) {
        String uri = mNetherApiBaseUri + "/" + URI_LEADERBOARDS + "/" + name;
        HttpOperationAsyncTask httpOperationAsyncTask =
                new HttpOperationAsyncTask(this, HttpOperationAsyncTask.HttpOperationType.GET);
        httpOperationAsyncTask.execute(uri);
    }

    /**
     * PPosts a new score of currently logged in player.
     *
     * @param country The country.
     * @param score The score.
     */
    public void postScoreForCurrentPlayer(String country, long score) {
        String uri = mNetherApiBaseUri + "/" + URI_SCORES;
        JSONObject jsonObject = new JSONObject();
        boolean success = false;

        try {
            jsonObject.put(ID_COUNTRY, country);
            jsonObject.put(ID_SCORE, score);
            success = true;
        } catch (JSONException e) {
            Log.e(TAG, e.getMessage(), e);
        }

        if (success) {
            HttpOperationAsyncTask httpOperationAsyncTask =
                    new HttpOperationAsyncTask(this, HttpOperationAsyncTask.HttpOperationType.POST);
            httpOperationAsyncTask.execute(uri, jsonObject.toString(), NetherOAuthManager.getAccessToken());
        }
    }

    /*
     * Admin methods ->
     */

    /**
     * Retrieves the details of the given user.
     *
     * @param userId The ID of the user whose details to get.
     */
    public void getUser(String userId) {
        String uri = mNetherApiBaseUri + "/" + URI_IDENTITY_USERS + "/" + userId;
        HttpOperationAsyncTask httpOperationAsyncTask =
                new HttpOperationAsyncTask(this, HttpOperationAsyncTask.HttpOperationType.GET);
        httpOperationAsyncTask.execute(uri);
    }

    /**
     * Retrieves the list of all users.
     */
    public void getUserList() {
        String uri = mNetherApiBaseUri + "/" + URI_IDENTITY_USERS;
        HttpOperationAsyncTask httpOperationAsyncTask =
                new HttpOperationAsyncTask(this, HttpOperationAsyncTask.HttpOperationType.GET);
        httpOperationAsyncTask.execute(uri);
    }

    /**
     * Adds a user with the given details.
     *
     * @param userRole Specifies the user's role.
     * @param isActive Specifies whether the user is active (i.e. should be allowed to log in).
     */
    public void addUser(UserRole userRole, boolean isActive) {
        String uri = mNetherApiBaseUri + "/" + URI_IDENTITY_USERS;
        JSONObject jsonObject = new JSONObject();
        String userRoleAsString = null;
        boolean success = false;

        switch (userRole) {
            case ADMIN: userRoleAsString = USER_ROLE_ADMIN; break;
            case PLAYER: userRoleAsString = USER_ROLE_PLAYER; break;
        }

        try {
            jsonObject.put(ID_ROLE, userRoleAsString);
            jsonObject.put(ID_ACTIVE, isActive);
            success = true;
        } catch (JSONException e) {
            Log.e(TAG, e.getMessage(), e);
        }

        if (success) {
            HttpOperationAsyncTask httpOperationAsyncTask =
                    new HttpOperationAsyncTask(this, HttpOperationAsyncTask.HttpOperationType.POST);
            httpOperationAsyncTask.execute(uri, jsonObject.toString());
        }
    }

    /*
     * <- Admin methods
     */

    /**
     * From NetherOAuthManager.Listener.
     *
     * @param accessToken The access token.
     */
    @Override
    public void onAccessTokenReceived(String accessToken) {
        if (mListener != null) {
            mListener.onAuthenticated(accessToken);
        }
    }

    /**
     * From NetherOAuthManager.Listener.
     *
     * @param errorMessage The error message.
     */
    @Override
    public void onAuthenticationFailed(String errorMessage) {
        if (mListener != null) {
            mListener.onAuthenticationFailed(errorMessage);
        }
    }

    /**
     * From HttpOperationAsyncTask.Listener.
     * Handles the received response.
     *
     * @param response The received response (from HTTP operations).
     */
    @Override
    public void onHttpOperationResponse(Response response) {
        mLatestNetherApiCallResponse = new NetherApiCallResponse(null, response.message(), response.code());
        ReadResponseAsyncTask readResponseAsyncTask = new ReadResponseAsyncTask(this);
        readResponseAsyncTask.execute(response);
    }

    /**
     * From ReadResponseAsyncTask.Listener.
     * Updates the latest API call response instance with the content and notifies the listener.
     *
     * @param bodyContent The body content as string.
     */
    @Override
    public void onBodyContentRead(String bodyContent) {
        if (mLatestNetherApiCallResponse != null) {
            mLatestNetherApiCallResponse.setContent(bodyContent);

            if (mListener != null) {
                mListener.onNetherApiCallResult(mLatestNetherApiCallResponse);
            }
        } else {
            if (mListener != null) {
                mListener.onNetherApiCallFailed("Invalid/corrupt response instance");
            }
        }
    }
}
