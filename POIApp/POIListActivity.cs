﻿using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Locations;

namespace POIApp
{
  static class GlobalApp
  {
    public static string TAG = "POIApp";
  }
  
  [Activity(Label = "POIs", MainLauncher = true)]
  public class POIListActivity : Activity, ILocationListener
  {
    ListView _poiListView;
    POIListViewAdapter _adapter;
    LocationManager _locMgr;

    protected override void OnCreate(Bundle bundle)
    {
      //Call SuperClass OnCreate
      base.OnCreate(bundle);
      Log.Info(GlobalApp.TAG, "OnCreate()");

      // Set our view from the "POIList" layout resource
      SetContentView(Resource.Layout.POIList);

      // request an LocationService instance
      _locMgr = GetSystemService(Context.LocationService) as LocationManager;

      // Hooking up POIListViewAdapter
      _poiListView = FindViewById<ListView>(Resource.Id.poiListView);
      _adapter = new POIListViewAdapter(this);
      _poiListView.Adapter = _adapter;

      //hook _poiListView up the event handler.
      _poiListView.ItemClick += POIClicked;
    }

    // Called after the activity has been stopped, just prior to it being started again. Always followed by onStart()
    protected override void OnRestart()
    {
      base.OnRestart();
      Log.Info(GlobalApp.TAG, "OnRestart()");
    }

    // The activity is about to become visible.
    protected override void OnStart()
    {
      base.OnStart();
      Log.Info(GlobalApp.TAG, "OnStart()");
    }

    // The activity has become visible (it is now "resumed").
    protected override void OnResume()
    {
      base.OnResume();
      Log.Info(GlobalApp.TAG, "OnResume()");

      //Notify BaseAdapter<> of Data Changes
      _adapter.NotifyDataSetChanged();

      // Specifying the criteria for the desired Location Provider
      Criteria criteria = new Criteria();
      criteria.Accuracy = Accuracy.NoRequirement;
      criteria.PowerRequirement = Power.NoRequirement;

      string provider = _locMgr.GetBestProvider(criteria, true);

      _locMgr.RequestLocationUpdates(provider, 20000, 100, this);
    }

    // Another activity is taking focus (this activity is about to be "paused").
    protected override void OnPause()
    {
      base.OnPause();
      Log.Info(GlobalApp.TAG, "OnPause()");

      // Remove Location Provider Remove Updates
      _locMgr.RemoveUpdates(this);
    }

    // The activity is no longer visible (it is now "stopped")
    protected override void OnStop()
    {
      base.OnStop();
      Log.Info(GlobalApp.TAG, "OnStop()");
    }

    // The activity is about to be destroyed.
    protected override void OnDestroy()
    {
      base.OnDestroy();
      Log.Info(GlobalApp.TAG, "OnDestroy()");
    }

    // The OnCreateOptionsMenu() method is called to give an opportunity 
    // to the Activity parameter to define actions for the ActionBar.
    public override bool OnCreateOptionsMenu(IMenu menu)
    {
      MenuInflater.Inflate(Resource.Menu.POIListViewMenu, menu);
      return base.OnCreateOptionsMenu(menu);
    }

    public override bool OnOptionsItemSelected(IMenuItem item)
    {
      switch (item.ItemId)
      {
        case Resource.Id.actionNew:
          StartActivity(typeof(POIDetailActivity));
          return true;

        case Resource.Id.actionRefresh:
          // The POIJsonService.RefreshCache() method is called to refresh the 
          // cache with the *.json files stored locally on the device.
          POIData.Service.RefreshCache();
          // The _adapter.NotifyDataSetChanged method is called so that poiListView
          // will be refreshed based on the updated cache of POIs.
          _adapter.NotifyDataSetChanged();
          return true;

        default:
          return base.OnOptionsItemSelected(item);
      }
    }

    //Process ListView Item Clicks
    protected void POIClicked(object sender, ListView.ItemClickEventArgs e)
    {
      //PointOfInterest poi = POIData.Service.GetPOI((int) e.Id);
      //Console.WriteLine("POIClicked: Id: {0}, Name: {1}", poi.Id, poi.Name);

      // setup the intent to pass the POI id to the detail view
      Intent poiDetailIntent = new Intent(this, typeof(POIDetailActivity));
      poiDetailIntent.PutExtra("poiId", (int) e.Id);
      StartActivity(poiDetailIntent);
    }

    #region ILocationListener implementation

    public void OnLocationChanged(Location location)
    {
      // Set CurrentLocation on POIListViewAdapter when a location change is received
      _adapter.CurrentLocation = location;
      // call NotifyDataSetChange() to cause the ListView to be refreshed
      _adapter.NotifyDataSetChanged();
    }

    public void OnProviderDisabled(string provider)
    {
    }

    public void OnProviderEnabled(string provider)
    {
    }

    public void OnStatusChanged(string provider, Availability status, Bundle extras)
    {
    }

    #endregion
  }
}
