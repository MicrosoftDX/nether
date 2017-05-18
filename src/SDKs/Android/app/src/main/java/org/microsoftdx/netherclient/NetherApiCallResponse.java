/* Copyright (c) 2017 Microsoft Corporation. This software is licensed under the MIT License.
 * See the license file delivered with this project for further information.
 */
package org.microsoftdx.netherclient;

/**
 * This is a Nether API call response.
 */
public class NetherApiCallResponse {
    private String mContent;
    private String mMessage;
    private int mResponseCode;

    /**
     * Constructor.
     *
     * @param content The response content.
     * @param message The response message.
     * @param responseCode The response code.
     */
    public NetherApiCallResponse(String content, String message, int responseCode) {
        mContent = content;
        mMessage = message;
        mResponseCode = responseCode;
    }

    public String getContent() {
        return mContent;
    }

    /**
     * Sets the content. Setter provided since sometimes reading the content is asynchronous and thus it may not be
     * possible to construct and set all the values of this class at once.
     *
     * @param content The content to set.
     */
    public void setContent(String content) {
        mContent = content;
    }

    public String getMessage() {
        return mMessage;
    }

    public int getResponseCode() {
        return mResponseCode;
    }

    @Override
    public String toString() {
        String retval = "[" + mResponseCode + " " + mMessage;

        if (mContent != null && mContent.length() > 0) {
            retval += " " + mContent;
        }

        retval += "]";
        return retval;
    }
}
