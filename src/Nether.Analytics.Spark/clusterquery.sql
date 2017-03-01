--get the frequency of hotspots for each workday

select DATENAME(dw, begintime) as dw, left(geohash8,6), count(*) from dbo.oldtown
group by DATENAME(dw, begintime), left(geohash8,6) order by dw
