﻿//using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Animation;
using TopMovies.Views;
using TopMovies.Common;


namespace TopMovies.Views
{
    public class CoverFlowProperties:NotificationObject
    {

        public CoverFlowProperties()
        {
            //Images = new ObservableCollection<Person>();

            //for (int iCount = 1; iCount <= 24; iCount++)
            //{
            //    if (iCount == 10)
            //        continue;
            //    images.Add(new Person() { Image = "ms-appx:///Assets/" + iCount + ".jpg" });
            //}
        }

        

        //public ObservableCollection<Person> Images
        //{
        //    get { return images; }
        //    set { images = value; }
        //}

        //public double Offset
        //{
        //    get { return offset; }
        //    set { offset = value; RaisePropertyChanged("Offset"); }
        //}

        //public double SelectedItemOffset
        //{
        //    get { return selecteditemoffset; }
        //    set { selecteditemoffset = value; RaisePropertyChanged("SelectedItemOffset"); }
        //}

        //public double RotationAngle
        //{
        //    get { return rotationangle; }
        //    set { rotationangle = value; RaisePropertyChanged("RotationAngle"); }
        //}

        //public double ZOffset
        //{
        //    get { return zoffset; }
        //    set { zoffset = value; RaisePropertyChanged("ZOffset"); }
        //}

        //public double ScaleOffset
        //{
        //    get { return scaleoffset; }
        //    set { scaleoffset = value; RaisePropertyChanged("ScaleOffset"); }
        //}
    }


    public class Person
    {
        public string Name { get; set; }

        public string Image { get; set; }

        public string Language { get; set; }

        public string Genere { get; set; }

        public int imdbID { get; set; }

        public Person()
        { }

        public Person(string name, string image, string title)
        {
            Name = name;
            Image = image;

        }
    }
}
