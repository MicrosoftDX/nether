/* Copyright (c) 2017 Microsoft Corporation. This software is licensed under the MIT License.
 * See the license file delivered with this project for further information.
 */
package org.microsoftdx.netherclient.networking;

import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;
import java.io.IOException;

/**
 * This class implements basic HTTP operations.
 */
public class HttpClientManager {
    public static final MediaType MEDIA_TYPE_JSON_UTF8 = MediaType.parse("application/json; charset=utf-8");
    public static final MediaType MEDIA_TYPE_WWW_FORM_URLENCODED_UTF8 = MediaType.parse("application/x-www-form-urlencoded; charset=utf-8");
    private static final String ID_AUTHORIZATION = "Authorization";
    private static final String BEARER = "Bearer";

    private OkHttpClient mHttpClient;

    /**
     * Constructor.
     */
    public HttpClientManager() {
        mHttpClient = new OkHttpClient();
    }

    /**
     * Executes a HTTP GET operation.
     *
     * @param url The URL.
     * @return The received response.
     * @throws IOException
     */
    public Response get(String url) throws IOException {
        Request request = new Request.Builder().url(url).build();
        return mHttpClient.newCall(request).execute();
    }

    /**
     * Executes a HTTP GET operation.
     *
     * @param url The URL.
     * @param accessToken The access token for the header.
     * @return The received response.
     * @throws IOException
     */
    public Response get(String url, String accessToken) throws IOException {
        Request request = new Request.Builder().url(url).build();
        return mHttpClient.newCall(request).execute();
    }

    /**
     * Executes a HTTP POST operation.
     *
     * @param url The URL.
     * @param jsonContent The JSON content to post.
     * @return The received response.
     * @throws IOException
     */
    public Response postJson(String url, String jsonContent) throws IOException {
        RequestBody body = RequestBody.create(MEDIA_TYPE_JSON_UTF8, jsonContent);
        Request request = new Request.Builder().url(url).post(body).build();
        return mHttpClient.newCall(request).execute();
    }

    /**
     * Executes a HTTP POST operation.
     *
     * @param url The URL.
     * @param jsonContent The JSON content to post.
     * @param accessToken The access token for the header.
     * @return The received response.
     * @throws IOException
     */
    public Response postJson(String url, String jsonContent, String accessToken) throws IOException {
        RequestBody body = RequestBody.create(MEDIA_TYPE_JSON_UTF8, jsonContent);
        Request request = new Request.Builder().url(url).addHeader(ID_AUTHORIZATION, BEARER + " " + accessToken).post(body).build();
        return mHttpClient.newCall(request).execute();
    }

    /**
     * Executes a HTTP POST operation.
     *
     * @param url The URL.
     * @param bodyContent The body content to post.
     * @return The received response.
     * @throws IOException
     */
    public Response postForm(String url, String bodyContent) throws IOException {
        RequestBody body = RequestBody.create(MEDIA_TYPE_WWW_FORM_URLENCODED_UTF8, bodyContent);
        Request request = new Request.Builder().url(url).post(body).build();
        return mHttpClient.newCall(request).execute();
    }
}
