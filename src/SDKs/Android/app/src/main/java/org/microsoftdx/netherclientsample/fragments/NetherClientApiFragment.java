/* Copyright (c) 2017 Microsoft Corporation. This software is licensed under the MIT License.
 * See the license file delivered with this project for further information.
 */
package org.microsoftdx.netherclientsample.fragments;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.*;
import org.microsoftdx.netherclient.NetherClient;
import org.microsoftdx.netherclientsample.MainActivity;
import org.microsoftdx.netherclientsample.R;
import org.microsoftdx.netherclientsample.models.Settings;

/**
 * A fragment for changing the application settings and testing the Nether client API.
 */
public class NetherClientApiFragment extends Fragment {
    private static final String TAG = NetherClientApiFragment.class.getSimpleName();
    private Settings mSettings = null;
    private EditText mNetherBaseUrldEditText = null;
    private EditText mNetherClientIdEditText = null;
    private EditText mNetherClientSecretEditText = null;
    private EditText mUserIdEditText = null;
    private EditText mPasswordEditText = null;
    private EditText mCountryEditText = null;
    private EditText mScoreEditText = null;
    private EditText mLeaderboardNameEditText = null;
    private Button mAuthenticateButton = null;
    private Button mPostScoreButton = null;
    private Button mGetLeaderboardButton = null;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_nether_client_api, container, false);

        mSettings = Settings.getInstance(view.getContext());
        mSettings.load();

        mNetherBaseUrldEditText = (EditText) view.findViewById(R.id.netherBaseUrlEditText);
        mNetherBaseUrldEditText.setText(mSettings.getNetherBaseUrl());
        mNetherBaseUrldEditText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {
            }

            @Override
            public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {
            }

            @Override
            public void afterTextChanged(Editable editable) {
                try {
                    mSettings.setNetherBaseUrl(editable.toString());
                } catch (NumberFormatException e) {
                    Log.e(TAG, e.getMessage(), e);
                    mNetherBaseUrldEditText.setText(mSettings.getNetherBaseUrl());
                }
            }
        });

        mNetherClientIdEditText = (EditText) view.findViewById(R.id.netherClientIdEditText);
        mNetherClientIdEditText.setText(mSettings.getNetherClientId());
        mNetherClientIdEditText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {
            }

            @Override
            public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {
            }

            @Override
            public void afterTextChanged(Editable editable) {
                try {
                    mSettings.setNetherClientId(editable.toString());
                } catch (NumberFormatException e) {
                    Log.e(TAG, e.getMessage(), e);
                    mNetherClientIdEditText.setText(mSettings.getNetherClientId());
                }
            }
        });

        mNetherClientSecretEditText = (EditText) view.findViewById(R.id.netherClientSecretEditText);
        mNetherClientSecretEditText.setText(mSettings.getNetherClientSecret());
        mNetherClientSecretEditText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {
            }

            @Override
            public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {
            }

            @Override
            public void afterTextChanged(Editable editable) {
                try {
                    mSettings.setNetherClientSecret(editable.toString());
                } catch (NumberFormatException e) {
                    Log.e(TAG, e.getMessage(), e);
                    mNetherClientSecretEditText.setText(mSettings.getNetherClientSecret());
                }
            }
        });

        mUserIdEditText = (EditText) view.findViewById(R.id.userIdEditText);
        mUserIdEditText.setText(mSettings.getUserId());
        mUserIdEditText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {
            }

            @Override
            public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {
            }

            @Override
            public void afterTextChanged(Editable editable) {
                try {
                    mSettings.setUserId(editable.toString());
                } catch (NumberFormatException e) {
                    Log.e(TAG, e.getMessage(), e);
                    mUserIdEditText.setText(mSettings.getUserId());
                }
            }
        });

        mPasswordEditText = (EditText) view.findViewById(R.id.passwordEditText);
        mPasswordEditText.setText(mSettings.getPassword());
        mPasswordEditText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {
            }

            @Override
            public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {
            }

            @Override
            public void afterTextChanged(Editable editable) {
                try {
                    mSettings.setPassword(editable.toString());
                } catch (NumberFormatException e) {
                    Log.e(TAG, e.getMessage(), e);
                    mPasswordEditText.setText(mSettings.getPassword());
                }
            }
        });

        mCountryEditText = (EditText) view.findViewById(R.id.countryEditText);
        mCountryEditText.setText(mSettings.getCountry());
        mCountryEditText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {
            }

            @Override
            public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {
            }

            @Override
            public void afterTextChanged(Editable editable) {
                try {
                    mSettings.setCountry(editable.toString());
                } catch (NumberFormatException e) {
                    Log.e(TAG, e.getMessage(), e);
                    mCountryEditText.setText(mSettings.getCountry());
                }
            }
        });

        mScoreEditText = (EditText) view.findViewById(R.id.scoreEditText);
        mScoreEditText.setText(String.valueOf(mSettings.getScore()));
        mScoreEditText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {
            }

            @Override
            public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {
            }

            @Override
            public void afterTextChanged(Editable editable) {
                if (editable.length() > 0) {
                    try {
                        mSettings.setScore(Long.parseLong(editable.toString()));
                    } catch (NumberFormatException e) {
                        Log.e(TAG, e.getMessage(), e);
                        mScoreEditText.setText(String.valueOf(mSettings.getScore()));
                    }
                }
            }
        });

        mLeaderboardNameEditText = (EditText) view.findViewById(R.id.leaderboardNameEditText);
        mLeaderboardNameEditText.setText(mSettings.getLeaderboardName());
        mLeaderboardNameEditText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence charSequence, int i, int i1, int i2) {
            }

            @Override
            public void onTextChanged(CharSequence charSequence, int i, int i1, int i2) {
            }

            @Override
            public void afterTextChanged(Editable editable) {
                try {
                    mSettings.setLeaderboardName(editable.toString());
                } catch (NumberFormatException e) {
                    Log.e(TAG, e.getMessage(), e);
                    mLeaderboardNameEditText.setText(mSettings.getLeaderboardName());
                }
            }
        });

        mAuthenticateButton = (Button) view.findViewById(R.id.authenticateButton);
        mAuthenticateButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                NetherClient netherClient = MainActivity.getNetherClient();

                if (netherClient != null) {
                    netherClient.login(mSettings.getUserId(), mSettings.getPassword());
                } else {
                    logAndDisplayNoNetherClientError();;
                }
            }
        });

        mPostScoreButton = (Button) view.findViewById(R.id.postScoreButton);
        mPostScoreButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                NetherClient netherClient = MainActivity.getNetherClient();

                if (netherClient != null) {
                    netherClient.postScoreForCurrentPlayer(mSettings.getCountry(), mSettings.getScore());
                } else {
                    logAndDisplayNoNetherClientError();;
                }
            }
        });

        mGetLeaderboardButton = (Button) view.findViewById(R.id.getLeaderboardButton);
        mGetLeaderboardButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                NetherClient netherClient = MainActivity.getNetherClient();

                if (netherClient != null) {
                    netherClient.getLeaderboard(mSettings.getLeaderboardName());
                } else {
                    logAndDisplayNoNetherClientError();;
                }
            }
        });

        return view;
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
    }

    private void logAndDisplayNoNetherClientError() {
        String message = "No Nether client created - make sure you have entered the URL, client ID and secret";
        Log.e(TAG, message);
        LogFragment.logError(message);
        MainActivity.showToast(message);
    }
}
