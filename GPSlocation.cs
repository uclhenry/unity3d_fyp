using UnityEngine;
using System;
using System.Collections;

public class GPSlocation
{
	public double latitude;
	public double longitude;

	public GPSlocation(double x, double y)
	{
		latitude = x;
		longitude = y;
	}

	public GPSlocation(float x, float y)
	{
		latitude = (double)x;
		longitude = (double)y;
	}

	public static double Distance(GPSlocation a, GPSlocation b) 
	{
		double theta = a.longitude - b.longitude;

		double dist = Math.Sin(Mathf.Deg2Rad * a.latitude) * Math.Sin(Mathf.Deg2Rad * b.latitude) + 
					  Math.Cos(Mathf.Deg2Rad * a.latitude) * Math.Cos(Mathf.Deg2Rad * b.latitude) * Math.Cos( Mathf.Deg2Rad * theta);
		dist = Math.Acos(dist);
		dist = Mathf.Rad2Deg * dist;
		dist = dist * 60 * 1.1515;

		dist = dist * 1609.344; //m
//		   dist = dist * 1.609344; //km
//		   dist = dist * 0.8684;   //miles
		return dist;
		//I don't know why there is an offset of 90 degrees. Angle is counterclockwise
	}

	public static double Direction(GPSlocation a, GPSlocation b)
	{
		double dLatitude  = b.latitude - a.latitude;
		double dLongitude = b.longitude - a.longitude;

		double angle = System.Math.Atan2( dLatitude, System.Math.Cos(Mathf.PI/180*a.latitude) * dLongitude);

		angle = angle * 180 / Mathf.PI;

		return (angle -90); // convention: 0 degrees when it's vertical, positive counterclockwise
	}
}
