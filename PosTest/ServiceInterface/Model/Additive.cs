﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Windows.Media;
using Newtonsoft.Json;


namespace ServiceInterface.Model
{
    public class Additive : Ranked, IState<long> //, IEquatable<Additive>
    {
        private string _description;
        private string _backgroundString;
        private SolidColorBrush _background;
        private AdditiveState _state;
        private DateTime? _timeStamp;
        private bool _isFavorite;
        private long? _id;

        public Additive()
        {
        }

        public Additive(Additive additive)
        {
            Id = additive.Id;
            Description = additive.Description;
            BackgroundString = additive.BackgroundString;
            Rank = additive.Rank;
        }

        [DataMember] public long? Id { get => _id; set =>Set(ref  _id , value); }

        [DataMember]
        [Required(ErrorMessage = "Name must not be Null or Empty")]
        [MinLength(5, ErrorMessage = "Name must not be under 5 charac")]
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        public List<Ingredient> Ingrediants { get; set; }

        [DataMember] public List<long> IdIngrediants { get; set; }

        public OrderItem ParentOrderItem { get; set; }

        [DataMember]
        public string BackgroundString
        {
            get => _backgroundString ?? "#00f39c12";
            set => Set(ref _backgroundString, value);
        }

        public Brush Background
        {
            get => new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundString));

            set
            {
                Set(ref _background, (SolidColorBrush)value);
                if (value != null)
                {
                    Set(ref _backgroundString, _background.Color.ToString(), nameof(BackgroundString));
                }
                else
                {
                    Set(ref _backgroundString, null);
                }

                NotifyOfPropertyChange(nameof(IsDark));
                NotifyOfPropertyChange(nameof(Background));
            }
        }
        [DataMember]
        public bool IsFavorite { get => _isFavorite; set => Set(ref _isFavorite, value); }


        /*bool IEquatable<Additive>.Equals(Additive additive)
        {
            // var additive = additiveObj as Additive;
            var result = additive != null &&
                    additive.Id == Id &&
                    additive.Name == Name &&
                    additive.BackgroundString == BackgroundString;
            return result;
        }*/


        public override bool Equals(object other)
        {
            var result = other is Additive additive &&
                         additive.Id == Id &&
                         additive.Description == Description &&
                         additive.BackgroundString == BackgroundString &&
                         additive.Rank == Rank;
            return result;
            //           return Equals(other as Additive);
        }

        public bool IsDark
        {
            get
            {
                var c = ((Background as SolidColorBrush)?.Color).GetValueOrDefault();
                var d = (5 * c.G + 2 * c.R + c.B) <= 8 * 128;
                return (5 * c.G + 2 * c.R + c.B) <= 8 * 140;
            }
        }


        public Additive Clone()
        {
            return new Additive()
            {
                Background = Background,
                Description = Description,
                BackgroundString = BackgroundString,
                Id = Id,
                IdIngrediants = IdIngrediants,
                Ingrediants = Ingrediants,
                IsNotifying = IsNotifying,
                ParentOrderItem = ParentOrderItem,
                Rank = Rank,
                IsFavorite = IsFavorite
            };
        }
        public override string ToString()
        {
            return $"{Description}";
        }


    }


    public enum AdditiveState
    {
        Added,
        Removed
    }
}