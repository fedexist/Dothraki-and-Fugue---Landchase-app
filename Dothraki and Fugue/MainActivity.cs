﻿using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Content.Res;
using System.IO;
using Android.Util;
using Java.Util;
using System.Collections.Generic;
using Java.IO;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Views;
using System;

namespace Dothraki_and_Fugue {
    [Activity(Label = "Dothraki_and_Fugue", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Landscape)]


    public class MainActivity : Activity, GestureDetector.IOnGestureListener
    {
        GestureDetector gestureDetector;
        ImageView imageView;
        ImageButton imageButton;
        AssetManager assets;
        List<Card> Cards;
        int index = 0;
        int done = 0;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            assets = this.Assets;
            string[] content;
            content = assets.List("Cards");
            Cards = new List<Card>();// = ArrayList<Card>
            imageView = FindViewById<ImageView>(Resource.Id.imageView1);
            imageButton = FindViewById<ImageButton>(Resource.Id.imageButton1);
            gestureDetector = new GestureDetector(this);

            if (done == 1)
                return;

            foreach (string c in content)
            {
                Cards.Add(Card.myRegex(c));
            }

            int doOnce = 0;


            // get input stream
            System.IO.Stream ims = null;
            for (int i = 0; i < Cards.Count; i++)
            {
                //multiple a change of seasons
                ims = assets.Open("Cards/" + Cards[i].path);
                // load image as Drawable
                Cards[i].bm =  BitmapFactory.DecodeStream(ims);
                if (Cards[i].name == "A Change of Seasons" && doOnce == 0)
                {
                    doOnce++;
                    Cards.Add(new Card(Cards[i]));
                    Cards.Add(new Card(Cards[i]));
                    Cards.Add(new Card(Cards[i]));
                }
                //tie day and night
                if (Cards[i].isTransforming())
                {
                    TransformingPlane c_T = (TransformingPlane)Cards[i];
                    for (int j = i + 1; j < Cards.Count; j++)
                    {
                        TransformingPlane d_T;
                        if (Cards[j].isTransforming())
                        { 
                            d_T = (TransformingPlane)Cards[j];
                            ims = assets.Open("Cards/" + Cards[j].path);
                            Cards[j].bm = BitmapFactory.DecodeStream(ims);
                        }
                        else
                            continue;

                        if (c_T.sameId(d_T))
                        {
                            c_T.link(d_T);
                            if (c_T.dOrN)
                                Cards.Remove(Cards[j]);
                            else
                                Cards.Remove(Cards[i]);
                            continue;
                        }
                    }
                }
            }
            Cards = Randomize<Card>(Cards);

            
            // set image to ImageView
            
            imageView.SetScaleType(ImageView.ScaleType.FitCenter);
            imageView.SetImageBitmap(Cards[0].bm);

            if (Cards[index].isTransforming())
            {
                imageButton.Visibility = ViewStates.Visible;
                imageButton.SetImageResource(Resource.Drawable.d);
            }
            else
            {
                imageButton.Visibility = ViewStates.Invisible;
            }
            imageButton.Click += (sender, e) => {
                toggle((TransformingPlane)Cards[index]);
            };
            done = 1;

        }

        public static List<T> Randomize<T>(List<T> list)
        {
            List<T> randomizedList = new List<T>();
            System.Random rnd = new System.Random();
            while (list.Count > 0)
            {
                int index = rnd.Next(0, list.Count); //pick a random item from the master list
                randomizedList.Add(list[index]); //place it at the end of the randomized list
                list.RemoveAt(index);
            }
            return randomizedList;
        }

        public void toggle(TransformingPlane tp)
        {
            if (tp.dOrN)
            {
                imageButton.SetImageResource(Resource.Drawable.n);
                imageButton.Click -= (sender, e) => {
                    toggle((TransformingPlane)Cards[index]);
                };
                imageButton.Click -= (sender, e) => {
                    toggle(tp.d);
                };
                imageButton.Click += (sender, e) => {
                    toggle(tp.n);
                };
                imageView.SetImageBitmap(tp.n.bm);
            }
            else
            {
                imageButton.SetImageResource(Resource.Drawable.d);
                imageButton.Click += (sender, e) => {
                    toggle(tp.d);
                };
                imageButton.Click -= (sender, e) => {
                    toggle(tp.n);
                };
                imageView.SetImageBitmap(tp.d.bm);
            }
            
        }

        public bool OnDown(MotionEvent e)
        {
            return false;
        }

        public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
           if(e1.RawX-e2.RawX < 0)
            {
                if (index > 0)
                {
                    index--;
                    imageView.SetImageBitmap(Cards[index].bm);
                }
            }
           else
            {
                if (index < Cards.Count-1)
                {
                    index++;
                    imageView.SetImageBitmap(Cards[index].bm);
                }
            }
            if (Cards[index].isTransforming())
            {
                imageButton.Visibility = ViewStates.Visible;
                imageButton.SetImageResource(Resource.Drawable.d);
            }
            else
            {
                imageButton.Visibility = ViewStates.Invisible;
            }
            return true;
        }

        public void OnLongPress(MotionEvent e)
        {
        }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            return false;
        }

        public void OnShowPress(MotionEvent e)
        {
        }

        public bool OnSingleTapUp(MotionEvent e)
        {
            return false;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            gestureDetector.OnTouchEvent(e);
            return false;
        }
    }

}

