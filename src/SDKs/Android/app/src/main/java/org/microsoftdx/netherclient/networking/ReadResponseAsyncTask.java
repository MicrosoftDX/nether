/* Copyright (c) 2017 Microsoft Corporation. This software is licensed under the MIT License.
 * See the license file delivered with this project for further information.
 */
package org.microsoftdx.netherclient.networking;

import android.os.AsyncTask;
import android.util.Log;
import okhttp3.Response;
import java.io.IOException;

/**
 * Async task for reading the body content of a response.
 */
public class ReadResponseAsyncTask extends AsyncTask<Response, Void, String> {
    public interface Listener {
        void onBodyContentRead(String bodyContent);
    }

    private static final String TAG = ReadResponseAsyncTask.class.getSimpleName();
    private Listener mListener;

    public ReadResponseAsyncTask(Listener listener) {
        mListener = listener;
    }

    @Override
    protected String doInBackground(Response... responses) {
        Response response = responses[0];
        String readBodyContent = null;

        if (response != null) {
            byte[] bodyContentAsByteArray = null;

            try {
                bodyContentAsByteArray = response.body().bytes();
            } catch (IOException | NullPointerException e) {
                Log.e(TAG, e.getMessage(), e);
            }

            if (bodyContentAsByteArray != null) {
                readBodyContent = new String(bodyContentAsByteArray);

                if (readBodyContent.length() > 0) {
                    Log.d(TAG, "Read: " + readBodyContent);
                } else {
                    Log.d(TAG, "No body content");
                }
            } else {
                Log.e(TAG, "Null byte array");
            }
        } else {
            Log.e(TAG, "Response is null");
        }

        return readBodyContent;
    }

    @Override
    protected void onPostExecute(String readBodyContent) {
        if (mListener != null) {
            mListener.onBodyContentRead(readBodyContent);
        }
    }
}
