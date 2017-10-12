/* Copyright (c) 2017 Microsoft Corporation. This software is licensed under the MIT License.
 * See the license file delivered with this project for further information.
 */
package org.microsoftdx.netherclientsample.models;

import android.content.Context;
import android.content.SharedPreferences;
import android.preference.PreferenceManager;
import android.util.Log;
import org.microsoftdx.netherclientsample.MainActivity;

/**
 * Manages the application settings.
 */
public class Settings {
    private static final String TAG = Settings.class.getName();
    private static Settings mInstance = null;
    private static Context mContext = null;
    private final SharedPreferences mSharedPreferences;
    private final SharedPreferences.Editor mSharedPreferencesEditor;

    private static final String KEY_NETHER_BASE_URL = "nether_base_url";
    private static final String KEY_NETHER_CLIENT_ID = "nether_client_id";
    private static final String KEY_NETHER_CLIENT_SECRET = "nether_client_secret";
    private static final String KEY_USER_ID = "user_id";
    private static final String KEY_PASSWORD = "password";
    private static final String KEY_COUNTRY = "country";
    private static final String KEY_LEADERBOARD_NAME = "leaderboard_name";
    private static final String KEY_SCORE = "score";

    private String mNetherBaseUrl;
    private String mNetherClientId;
    private String mNetherClientSecret;
    private String mUserId;
    private String mPassword;
    private String mCountry;
    private String mLeaderboardName;
    private long mScore;
    private boolean mLoaded = false;

    /**
     * Returns the singleton instance of this class. Creates the instance if not already created.
     *
     * @param context The application context.
     * @return The singleton instance of this class or null, if no context given and not created before.
     */
    public static Settings getInstance(Context context) {
        if (mInstance == null && context != null) {
            mContext = context;
            mInstance = new Settings(mContext);
        }

        return mInstance;
    }

    /**
     * Constructor.
     *
     * @param context The application context.
     */
    private Settings(Context context) {
        mSharedPreferences = PreferenceManager.getDefaultSharedPreferences(context);
        mSharedPreferencesEditor = mSharedPreferences.edit();
    }

    /**
     * Loads the settings.
     */
    public void load() {
        if (!mLoaded) {
            mLoaded = true;
            mNetherBaseUrl = mSharedPreferences.getString(KEY_NETHER_BASE_URL, "");
            mNetherClientId = mSharedPreferences.getString(KEY_NETHER_CLIENT_ID, "");
            mNetherClientSecret = mSharedPreferences.getString(KEY_NETHER_CLIENT_SECRET, "");
            mUserId = mSharedPreferences.getString(KEY_USER_ID,"");
            mPassword = mSharedPreferences.getString(KEY_PASSWORD, "");
            mCountry = mSharedPreferences.getString(KEY_COUNTRY, "");
            mLeaderboardName = mSharedPreferences.getString(KEY_LEADERBOARD_NAME, "");
            mScore = mSharedPreferences.getLong(KEY_SCORE, 0);

            Log.d(TAG, "load: "
                    + "\nNether base URL: " + mNetherBaseUrl
                    + "\nNether client ID: " + mNetherClientId
                    + "\nNether client secret: " + mNetherClientSecret
                    + "\nUser ID: " + mUserId
                    + "\nPassword: " + mPassword
                    + "\nCountry: " + mCountry
                    + "\nScore: " + String.valueOf(mScore)
                    + "\nLeaderboard name: " + mLeaderboardName
            );

            createNetherClientIfSettingsLookValid();
        } else {
            Log.v(TAG, "load: Already loaded");
        }
    }

    public String getNetherBaseUrl() {
        return mNetherBaseUrl;
    }

    public void setNetherBaseUrl(String netherBaseUrl) {
        Log.d(TAG, "setNetherBaseUrl: " + netherBaseUrl);
        mNetherBaseUrl = netherBaseUrl;
        mSharedPreferencesEditor.putString(KEY_NETHER_BASE_URL, mNetherBaseUrl);
        mSharedPreferencesEditor.apply();
        createNetherClientIfSettingsLookValid();
    }

    public String getNetherClientId() {
        return mNetherClientId;
    }

    public void setNetherClientId(String netherClientId) {
        Log.d(TAG, "setNetherClientId: " + netherClientId);
        mNetherClientId = netherClientId;
        mSharedPreferencesEditor.putString(KEY_NETHER_CLIENT_ID, mNetherClientId);
        mSharedPreferencesEditor.apply();
        createNetherClientIfSettingsLookValid();
    }

    public String getNetherClientSecret() {
        return mNetherClientSecret;
    }

    public void setNetherClientSecret(String netherClientSecret) {
        Log.d(TAG, "setNetherClientSecret: " + netherClientSecret);
        mNetherClientSecret = netherClientSecret;
        mSharedPreferencesEditor.putString(KEY_NETHER_CLIENT_SECRET, mNetherClientSecret);
        mSharedPreferencesEditor.apply();
        createNetherClientIfSettingsLookValid();
    }

    public String getUserId() {
        return mUserId;
    }

    public void setUserId(String userId) {
        Log.d(TAG, "setUserId: " + userId);
        mUserId = userId;
        mSharedPreferencesEditor.putString(KEY_USER_ID, mUserId);
        mSharedPreferencesEditor.apply();
    }

    public String getPassword() {
        return mPassword;
    }

    public void setPassword(String password) {
        Log.d(TAG, "setPassword: " + password);
        mPassword = password;
        mSharedPreferencesEditor.putString(KEY_PASSWORD, mPassword);
        mSharedPreferencesEditor.apply();
    }

    public String getCountry() {
        return mCountry;
    }

    public void setCountry(String country) {
        Log.d(TAG, "setCountry: " + country);
        mCountry = country;
        mSharedPreferencesEditor.putString(KEY_COUNTRY, mCountry);
        mSharedPreferencesEditor.apply();
    }

    public String getLeaderboardName() {
        return mLeaderboardName;
    }

    public void setLeaderboardName(String leaderboardName) {
        Log.d(TAG, "setLeaderboardName: " + leaderboardName);
        mLeaderboardName = leaderboardName;
        mSharedPreferencesEditor.putString(KEY_LEADERBOARD_NAME, mLeaderboardName);
        mSharedPreferencesEditor.apply();
    }

    public long getScore() {
        return mScore;
    }

    public void setScore(long score) {
        Log.d(TAG, "setScore: " + score);
        mScore = score;
        mSharedPreferencesEditor.putLong(KEY_SCORE, mScore);
        mSharedPreferencesEditor.apply();
    }

    /**
     * Creates a new Nether client instance, if the required settings look valid (non-empty strings).
     */
    private void createNetherClientIfSettingsLookValid() {
        if (mNetherBaseUrl != null && mNetherBaseUrl.length() > 0
                && mNetherClientId != null && mNetherClientId.length() > 0
                && mNetherClientSecret != null && mNetherClientSecret.length() > 0) {
            MainActivity.createNetherClient(mNetherBaseUrl, mNetherClientId, mNetherClientSecret);
        }
    }
}
