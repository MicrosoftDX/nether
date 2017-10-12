/* Copyright (c) 2017 Microsoft Corporation. This software is licensed under the MIT License.
 * See the license file delivered with this project for further information.
 */
package org.microsoftdx.netherclient.networking;

import android.os.AsyncTask;
import android.util.Log;
import okhttp3.MediaType;
import okhttp3.Response;
import java.io.IOException;

/**
 * Async task for HTTP operations.
 */
public class HttpOperationAsyncTask extends AsyncTask<String, Void, Response> {
    /**
     * The listener for operation results.
     */
    public interface Listener {
        void onHttpOperationResponse(Response response);
    }

    /**
     * HTTP operation types.
     */
    public enum HttpOperationType {
        GET,
        POST,
        DELETE
    }

    private static final String TAG = HttpOperationAsyncTask.class.getSimpleName();
    private static final int ACCESS_TOKEN_INDEX = 2;

    private Listener mListener;
    private HttpOperationType mHttpOperationType;
    private MediaType mMediaType;

    /**
     * Constructor.
     *
     * @param listener The listener.
     * @param httpOperationType The HTTP operation type.
     */
    public HttpOperationAsyncTask(Listener listener, HttpOperationType httpOperationType) {
        mListener = listener;
        mHttpOperationType = httpOperationType;
    }

    /**
     * Constructor.
     *
     * @param listener The listener.
     * @param httpOperationType The HTTP operation type.
     * @param mediaType The media type of the content to post.
     */
    public HttpOperationAsyncTask(Listener listener, HttpOperationType httpOperationType, MediaType mediaType) {
        this(listener, httpOperationType);
        mMediaType = mediaType;
    }

    /**
     * @return The HTTP operation type.
     */
    public HttpOperationType getHttpOperationType() {
        return mHttpOperationType;
    }

    /**
     * Sets the HTTP operation type.
     *
     * @param httpOperationType The desired HTTP operation type.
     */
    public void setHttpOperationType(HttpOperationType httpOperationType) {
        mHttpOperationType = httpOperationType;
    }

    /**
     * Executes a HTTP operation.
     *
     * @param strings [0]: URL; [1]: content (not required for GET); [2]: access token (optional)
     * @return The received response.
     */
    @Override
    protected Response doInBackground(String... strings) {
        Log.d(TAG, mHttpOperationType.toString() + " " + strings[0]);
        HttpClientManager httpClientManager = new HttpClientManager();
        String accessToken = null;

        if (strings.length > 2 && strings[ACCESS_TOKEN_INDEX] != null && strings[ACCESS_TOKEN_INDEX].length() > 0) {
            accessToken = strings[ACCESS_TOKEN_INDEX];
        }

        Response response = null;

        try {
            switch (mHttpOperationType) {
                case GET: {
                    if (accessToken != null) {
                        response = httpClientManager.get(strings[0], accessToken);
                    } else {
                        response = httpClientManager.get(strings[0]);
                    }
                    break;
                }
                case POST: {
                    if (mMediaType == HttpClientManager.MEDIA_TYPE_WWW_FORM_URLENCODED_UTF8) {
                        // No POST FORM with access token implemented
                        response = httpClientManager.postForm(strings[0], strings[1]);
                    } else {
                        // JSON is the default
                        if (accessToken != null) {
                            response = httpClientManager.postJson(strings[0], strings[1], accessToken);
                        } else {
                            response = httpClientManager.postJson(strings[0], strings[1]);
                        }
                    }

                    break;
                }
            }
        } catch (IOException | NullPointerException e) {
            Log.e(TAG, e.getMessage(), e);
        }

        return response;
    }

    /**
     * Relays the result response to the listener, if one exists.
     *
     * @param response The response received from the previous HTTP operation.
     */
    @Override
    protected void onPostExecute(Response response) {
        if (mListener != null) {
            mListener.onHttpOperationResponse(response);
        }
    }
}
