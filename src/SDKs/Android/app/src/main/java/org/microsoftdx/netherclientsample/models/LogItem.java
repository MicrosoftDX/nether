/* Copyright (c) 2015-2017 Microsoft Corporation. This software is licensed under the MIT License.
 * See the license file delivered with this project for further information.
 */
package org.microsoftdx.netherclientsample.models;

import java.sql.Timestamp;
import java.text.SimpleDateFormat;

/**
 * Contains data for a single log item.
 */
public class LogItem {
    public enum LogMessageType {
        NORMAL,
        ERROR,
        TEST
    }

    public Timestamp timestamp;
    public String timestampString = "";
    public String message = "";
    public LogMessageType type = LogMessageType.NORMAL;

    public LogItem(Timestamp timestamp, String message, LogMessageType type) {
        this.timestamp = timestamp;
        this.timestampString = new SimpleDateFormat("hh:mm:ss").format(timestamp);
        this.message = message;
        this.type = type;
    }
}
