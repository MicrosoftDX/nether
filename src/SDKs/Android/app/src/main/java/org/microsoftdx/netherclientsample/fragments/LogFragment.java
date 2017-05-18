/* Copyright (c) 2015-2017 Microsoft Corporation. This software is licensed under the MIT License.
 * See the license file delivered with this project for further information.
 */
package org.microsoftdx.netherclientsample.fragments;

import android.content.Context;
import android.content.res.ColorStateList;
import android.graphics.Color;
import android.os.Bundle;
import android.os.Handler;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ListView;
import android.widget.TextView;
import org.microsoftdx.netherclientsample.R;
import org.microsoftdx.netherclientsample.models.LogItem;
import java.sql.Timestamp;
import java.util.Date;
import java.util.concurrent.CopyOnWriteArrayList;

/**
 * A fragment managing and displaying the log items.
 */
public class LogFragment extends Fragment {
    private static final String TAG = LogFragment.class.getName();
    private static final int MAX_NUMBER_OF_LOG_ITEMS = 50;
    private static Context mContext = null;
    private static CopyOnWriteArrayList<LogItem> mLog = new CopyOnWriteArrayList<LogItem>();
    private static ListAdapter mListAdapter = null;
    private ListView mListView = null;
    private ColorStateList mDefaultTextViewColors = null;

    public LogFragment() {
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_log, container, false);

        mContext = view.getContext();
        mListAdapter = new ListAdapter(mContext);

        mListView = (ListView) view.findViewById(R.id.listView);
        mListView.setAdapter(mListAdapter);

        return view;
    }

    /**
     * Adds a new log item with the given message.
     *
     * @param message The message for the log item.
     */
    public static void logMessage(String message) {
        addLogItem(message, LogItem.LogMessageType.NORMAL);
    }

    /**
     * Adds a new log item with the given error message.
     *
     * @param errorMessage The error message for the log item.
     */
    public static void logError(String errorMessage) {
        addLogItem(errorMessage, LogItem.LogMessageType.ERROR);
    }

    /**
     * Adds a new log item with the given message.
     *
     * @param message        The message for the log item.
     * @param logMessageType The message type.
     */
    private static synchronized void addLogItem(String message, LogItem.LogMessageType logMessageType) {
        Timestamp timestamp = new Timestamp(new Date().getTime());
        final LogItem logItem = new LogItem(timestamp, message, logMessageType);

        if (mContext != null && mListAdapter != null) {
            Handler handler = new Handler(mContext.getMainLooper());

            handler.post(new Runnable() {
                @Override
                public void run() {
                    mLog.add(0, logItem);

                    if (mLog.size() > MAX_NUMBER_OF_LOG_ITEMS) {
                        mLog.remove(mLog.size() - 1); // Remove the last item
                    }
                    mListAdapter.notifyDataSetChanged();
                }
            });
        }
    }

    class ListAdapter extends BaseAdapter {
        private LayoutInflater mInflater = null;
        private Context mContext;

        public ListAdapter(Context context) {
            mContext = context;
            mInflater = (LayoutInflater) mContext
                    .getSystemService(Context.LAYOUT_INFLATER_SERVICE);
        }

        @Override
        public int getCount() {
            return mLog.size();
        }

        @Override
        public Object getItem(int position) {
            return mLog.get(position);
        }

        @Override
        public long getItemId(int position) {
            return position;
        }

        @Override
        public View getView(int position, View convertView, ViewGroup parent) {
            View view = convertView;

            if (view == null) {
                view = mInflater.inflate(R.layout.log_item, null);
            }

            LogItem logItem = mLog.get(position);

            TextView textView = (TextView) view.findViewById(R.id.timestamp);
            textView.setText(logItem.timestampString);

            if (mDefaultTextViewColors == null) {
                // Store the original colors
                mDefaultTextViewColors = textView.getTextColors();
            }

            if (logItem.type == LogItem.LogMessageType.ERROR) {
                textView.setTextColor(Color.RED);
            } else if (logItem.type == LogItem.LogMessageType.TEST) {
                textView.setTextColor(Color.BLUE);
            } else {
                textView.setTextColor(mDefaultTextViewColors);
            }

            textView = (TextView) view.findViewById(R.id.message);
            textView.setText(logItem.message);

            if (logItem.type == LogItem.LogMessageType.ERROR) {
                textView.setTextColor(Color.RED);
            } else if (logItem.type == LogItem.LogMessageType.TEST) {
                textView.setTextColor(Color.BLUE);
            } else {
                textView.setTextColor(mDefaultTextViewColors);
            }

            return view;
        }
    }
}
