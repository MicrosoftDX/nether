package org.microsoftdx.netherclientsample;

import android.content.Context;
import android.os.Handler;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentPagerAdapter;
import android.support.v4.view.ViewPager;
import android.support.v7.app.ActionBar;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.widget.Toast;
import org.microsoftdx.netherclient.NetherApiCallResponse;
import org.microsoftdx.netherclient.NetherClient;
import org.microsoftdx.netherclientsample.fragments.LogFragment;
import org.microsoftdx.netherclientsample.fragments.NetherClientApiFragment;
import org.microsoftdx.netherclientsample.slidingtabs.SlidingTabLayout;

/**
 * The main activity of the sample.
 */
public class MainActivity extends AppCompatActivity implements NetherClient.Listener {
    private static final String TAG = MainActivity.class.getName();
    private static MainActivity mInstance = null;
    private static Context mContext = null;
    private MyFragmentAdapter mMyFragmentAdapter;
    private ViewPager mViewPager;
    private SlidingTabLayout mSlidingTabLayout;
    private NetherClientApiFragment mNetherClientApiFragment;
    private LogFragment mLogFragment;

    private NetherClient mNetherClient;

    /**
     * @return The Nether client instance or null, if not created.
     */
    public static NetherClient getNetherClient() {
        if (mInstance != null) {
            return mInstance.mNetherClient;
        }

        return null;
    }

    /**
     * Creates the Nether client instance.
     *
     * @param netherBaseUrl The Nether base URL.
     * @param netherClientId The Nether client ID.
     * @param netherClientPassword The Nether client password.
     */
    public static void createNetherClient(String netherBaseUrl, String netherClientId, String netherClientPassword) {
        if (mInstance != null) {
            String netherBaseUrlTrimmed = netherBaseUrl.trim();
            Log.d(TAG, "Creating Nether client with parameters: " + netherBaseUrlTrimmed + " " + netherClientId + " " + netherClientPassword);
            mInstance.mNetherClient = new NetherClient(mInstance, netherBaseUrlTrimmed, netherClientId, netherClientPassword);
        } else {
            Log.e(TAG, "No MainActivity instance!");
        }
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        mInstance = this;
        mContext = getApplicationContext();

        final ActionBar actionBar = getSupportActionBar();

        mMyFragmentAdapter = new MyFragmentAdapter(getSupportFragmentManager(), this);

        mViewPager = (ViewPager) findViewById(R.id.pager);
        mViewPager.setAdapter(mMyFragmentAdapter);

        mSlidingTabLayout = (SlidingTabLayout) findViewById(R.id.sliding_tabs);
        mSlidingTabLayout.setViewPager(mViewPager);

        mLogFragment = new LogFragment();
        mNetherClientApiFragment = new NetherClientApiFragment();

        if (mNetherClient != null) {
            mNetherClient.setListener(this);
        }
    }

    @Override
    public void onNetherApiCallResult(NetherApiCallResponse netherApiCallResponse) {
        LogFragment.logMessage("Nether API call result: " + netherApiCallResponse.toString());
        showToast("Nether API call result: " + netherApiCallResponse.getResponseCode() + " " + netherApiCallResponse.getMessage());
    }

    @Override
    public void onNetherApiCallFailed(String errorMessage) {
        String message = "Nether API call failed: " + errorMessage;
        LogFragment.logError(message);
        showToast(message);
    }

    @Override
    public void onAuthenticated(String accessToken) {
        LogFragment.logMessage("Authenticated; access token: " + accessToken);
        showToast("User authenticated successfully!");
    }

    @Override
    public void onAuthenticationFailed(String errorMessage) {
        String message = "Authentication failed: " + errorMessage;
        LogFragment.logError(message);
        showToast(message);
    }

    /**
     * Displays a toast with the given message.
     *
     * @param message The message to show.
     */
    public static void showToast(final String message) {
        final Context context = mContext;

        if (context != null) {
            Handler handler = new Handler(mContext.getMainLooper());

            handler.post(new Runnable() {
                @Override
                public void run() {
                    CharSequence text = message;
                    int duration = Toast.LENGTH_SHORT;
                    Toast toast = Toast.makeText(context, text, duration);
                    toast.show();
                }
            });
        }
    }

    /**
     * The fragment adapter for tabs.
     */
    public class MyFragmentAdapter extends FragmentPagerAdapter {
        private static final int NETHER_CLIENT_API_FRAGMENT = 0;
        private static final int LOG_FRAGMENT = 1;
        private final MainActivity mMainActivity;

        public MyFragmentAdapter(FragmentManager fragmentManager, MainActivity mainActivity) {
            super(fragmentManager);
            mMainActivity = mainActivity;
        }

        @Override
        public Fragment getItem(int index) {
            switch (index) {
                case NETHER_CLIENT_API_FRAGMENT:
                    mNetherClientApiFragment = new NetherClientApiFragment();
                    //mNetherClientApiFragment.setListener(mMainActivity);
                    return mNetherClientApiFragment;
                case LOG_FRAGMENT:
                    return mLogFragment;
            }

            return null;
        }

        @Override
        public CharSequence getPageTitle(int position) {
            switch (position) {
                case NETHER_CLIENT_API_FRAGMENT:
                    return "Nether Client API";
                case LOG_FRAGMENT:
                    return "Log";
            }

            return super.getPageTitle(position);
        }

        @Override
        public int getCount() {
            return 2;
        }
    }
}
