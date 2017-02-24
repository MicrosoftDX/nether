#!/usr/bin/python

import pandas as pd, numpy as np, matplotlib.pyplot as plt, time
from sklearn.cluster import DBSCAN
from sklearn import metrics
from geopy.distance import great_circle
from shapely.geometry import MultiPoint
from collections import Counter
from datetime import datetime, timedelta
import Geohash

#inputs, TODO: if minPointsInCluster is greater than 2, noise will be labeld -1.  If we filter labels to be greater equal 0, then 
# there's a bug in numpy which will cause [coords[cluster_labels==n] for n in range(num_clusters)] to fail for index NaN
# so for now use minPointsInCluster 1
epsilonInMeters = 500
minPointsInCluster = 1

def get_centermost_point(cluster):
    centroid = (MultiPoint(cluster).centroid.x, MultiPoint(cluster).centroid.y)
    centermost_point = min(cluster, key=lambda point: great_circle(point, centroid).m)
    return tuple(centermost_point)

# define epsilon as kilometers, converted to radians for use by haversine
kms_per_radian = 6371.0088
epsilon = epsilonInMeters / float(1000) / kms_per_radian 
    
df = pd.read_csv('/tmp/janwalkers.csv', encoding='utf-8', sep='|')
# aggregate by the hour
df['eventtime'] = pd.to_datetime(df['enqueueTime']).apply(lambda dt: datetime(dt.year, dt.month, dt.day, dt.hour))
uniquehours = df['eventtime'].unique()
day = pd.to_datetime(df['eventtime'].iloc[0]).strftime("%Y-%m-%d")
alldf = []     
for curhour in uniquehours:
    curdf = df[df['eventtime'] == curhour]
    coords = curdf.as_matrix(columns=['lat', 'lon'])
    # print(coords)
    # run clustering algorithm to identify the clusters
    db = DBSCAN(eps=epsilon, min_samples=minPointsInCluster, algorithm='ball_tree', metric='haversine').fit(np.radians(coords))
    # the output the cluster id associated with each input item, non-cluster points (noise) is labeled as -1
    cluster_labels = db.labels_ 
    labeldf = pd.DataFrame({'clusterid': cluster_labels})
    # get the number of clusters
    num_clusters = len(set(cluster_labels))
    # turn the clusters in to a pandas series, where each element is a cluster of points, compute the point closest to the center of the cluster
    # this will break with 0-d boolean arrays will be interpreted as a valid boolean index, 
    # in that case, noise will be labeled as -1, and it can't find an element for index NaN
    clusters = pd.Series([coords[cluster_labels==n] for n in range(num_clusters)])
    centermost_points = clusters.map(get_centermost_point)
    lats, lons = zip(*centermost_points)
    rep_points = pd.DataFrame({'lon':lons, 'lat':lats})
    rep_points['geohash8'] = rep_points.apply(lambda x: Geohash.encode(x['lat'], x['lon'], 8), axis = 1)
    fulldf = curdf.reset_index().join(labeldf).sort_values(by=['clusterid'], ascending=1)
    # count the number of points in each cluster, order by cluster id
    # pointdf = pd.DataFrame.from_dict(Counter(cluster_labels), orient='index').reset_index().rename(columns={'index':'clusterid', 0:'numpoints'}).sort_values(by='clusterid', ascending = 1)
    pointSeries = fulldf.groupby('clusterid').size()
    # count the number of unique sessions in each cluster, order by cluster id
    sessionSeries = fulldf.groupby('clusterid')['gameSessionId'].nunique()
    # join the above series
    clusterdf = pd.concat([pointSeries, sessionSeries], axis =1)
    clusterdf.columns = ['numpoints', 'numuniquesessions']
    ts = pd.to_datetime(curhour)
    clusterdf['begintime'] = ts.strftime("%Y-%m-%d %H:%M:%S")
    clusterdf['endtime'] = (ts + timedelta(hours = 1)).strftime("%Y-%m-%d %H:%M:%S")
    # join the center points to get the final output [clusterid, numpoints, numuniquesessions, lat, lon]
    finaldf = clusterdf.join(rep_points).sort_values(by=['numpoints'], ascending=0)
    alldf.append(finaldf[finaldf['numpoints'] > 10])

pd.concat(alldf).to_csv('cluster_' + day + '.csv')

