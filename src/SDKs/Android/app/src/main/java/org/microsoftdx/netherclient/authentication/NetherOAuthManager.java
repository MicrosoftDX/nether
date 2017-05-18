/* Copyright (c) 2017 Microsoft Corporation. This software is licensed under the MIT License.
 * See the license file delivered with this project for further information.
 */
package org.microsoftdx.netherclient.authentication;

import android.util.Log;
import okhttp3.Response;
import org.json.JSONException;
import org.json.JSONObject;
import org.microsoftdx.netherclient.networking.HttpClientManager;
import org.microsoftdx.netherclient.networking.HttpOperationAsyncTask;
import org.microsoftdx.netherclient.networking.ReadResponseAsyncTask;

/**
 * This class implements a basic OAuth2 authentication for Nether.
 */
public class NetherOAuthManager implements HttpOperationAsyncTask.Listener, ReadResponseAsyncTask.Listener {
    /**
     * The listener for authentication results.
     */
    public interface Listener {
        void onAccessTokenReceived(String accessToken);
        void onAuthenticationFailed(String errorMessage);
    }

    private static final String TAG = NetherOAuthManager.class.getSimpleName();

    private static final String URI_IDENTITY = "identity";
    private static final String URI_ACCESS_TOKEN_ENDPOINT = URI_IDENTITY + "/connect/token";
    private static final String URI_AUTHORIZATION_ENDPOINT = URI_IDENTITY + "/connect/authorize";

    private static final String ID_ACCESS_TOKEN = "access_token";
    private static final String ID_CLIENT_ID = "client_id";
    private static final String ID_CLIENT_SECRET = "client_secret";
    private static final String ID_RESPONSE_TYPE = "response_type";
    private static final String ID_USERNAME = "username";
    private static final String ID_PASSWORD = "password";
    private static final String GRANT_TYPE_EQUALS_PASSWORD = "grant_type=password";

    private static final String RESPONSE_TYPE = "id_token token";
    private static final String SCOPE = "openid profile nether-all";

    private static final String AMPERSAND = "&";
    private static final String EQUALS = "=";

    private static final int RESPONSE_CODE_OK_200 = 200;
    private static final int RESPONSE_CODE_OK_201 = 201;

    private static String mAccessToken;
    private Listener mListener;
    private String mNetherBaseUrl;

    /**
     * Constructor.
     *
     * @param listener The listener.
     * @param netherBaseUrl The base URL of the Nether instance.
     */
    public NetherOAuthManager(Listener listener, String netherBaseUrl) {
        mListener = listener;
        mNetherBaseUrl = netherBaseUrl;
    }

    /**
     * @return The last received access token or null, if not authenticated yet.
     */
    public static String getAccessToken() {
        return mAccessToken;
    }

    /**
     * Initiates the authentication process for the given user.
     *
     * @param apiKey The Nether API key (client ID).
     * @param apiSecret The Nether API secret (client secret).
     * @param userId The ID of the user to authenticate.
     * @param password The password of the user to authenticate.
     */
    public void initiateAuthentication(String apiKey, String apiSecret, String userId, String password) {
        Log.d(TAG, "Authenticating user with ID \"" + userId + "\"");
        String accessTokenUri = getAccessTokenEndpoint();
        String bodyContent = createAccessTokenRequestBody(apiKey, apiSecret, userId, password);

        HttpOperationAsyncTask httpOperationAsyncTask = new HttpOperationAsyncTask(
                this,
                HttpOperationAsyncTask.HttpOperationType.POST,
                HttpClientManager.MEDIA_TYPE_WWW_FORM_URLENCODED_UTF8);
        httpOperationAsyncTask.execute(accessTokenUri, bodyContent);
    }

    /**
     * From HttpOperationAsyncTask.Listener.
     * Handles the received response and notifies the listener in case of a failure.
     *
     * @param response The received response to handle.
     */
    @Override
    public void onHttpOperationResponse(Response response) {
        if (mListener != null) {
            if (response != null) {
                if ((response.code() == RESPONSE_CODE_OK_200
                        || response.code() == RESPONSE_CODE_OK_201)
                        && response.body() != null) {
                    ReadResponseAsyncTask readResponseAsyncTask = new ReadResponseAsyncTask(this);
                    readResponseAsyncTask.execute(response);
                } else {
                    mListener.onAuthenticationFailed(response.code() + " " + response.message());
                }
            } else {
                mListener.onAuthenticationFailed("Failed to receive a response");
            }
        }
    }

    /**
     * From ReadResponseAsyncTask.Listener.
     * Parses the access token from the given content and notifies the listener.
     *
     * @param bodyContent The read body content.
     */
    @Override
    public void onBodyContentRead(String bodyContent) {
        JSONObject jsonObject = null;
        boolean success = false;

        try {
            jsonObject = new JSONObject(bodyContent);
            mAccessToken = jsonObject.getString(ID_ACCESS_TOKEN);
            success = true;
        } catch (JSONException e) {
            Log.e(TAG, "Failed to parse the body content: " + e.getMessage(), e);
        }

        if (mListener != null) {
            if (success && mAccessToken != null && mAccessToken.length() > 0) {
                mListener.onAccessTokenReceived(mAccessToken);
            } else {
                mListener.onAuthenticationFailed("Failed to parse the access token");
            }
        }
    }

    /**
     * @return The access token endpoint URI.
     */
    private String getAccessTokenEndpoint(){
        return mNetherBaseUrl + "/" + URI_ACCESS_TOKEN_ENDPOINT;
    }

    /**
     * Creates an access token request body content.
     *
     * @param apiKey The Nether API key (client ID).
     * @param apiSecret The Nether API secret (client secret).
     * @param userId The ID of the user to authenticate.
     * @param password The password of the user to authenticate.
     * @return A newly created access token URI.
     */
    private String createAccessTokenRequestBody(String apiKey, String apiSecret, String userId, String password) {
        String accessTokenRequestBodyContent =
                GRANT_TYPE_EQUALS_PASSWORD
                + AMPERSAND + ID_USERNAME + EQUALS + userId
                + AMPERSAND + ID_PASSWORD + EQUALS + password
                + AMPERSAND + ID_RESPONSE_TYPE + EQUALS + RESPONSE_TYPE
                + AMPERSAND + ID_CLIENT_ID + EQUALS + apiKey
                + AMPERSAND + ID_CLIENT_SECRET + EQUALS + apiSecret;

        Log.d(TAG, "Access token request body content: " + accessTokenRequestBodyContent);
        return accessTokenRequestBodyContent;
    }
}
