﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization;

#if METRO
using Windows.UI.Xaml.Data; 
using System.Reflection;
using System.ComponentModel;
using System.Runtime.CompilerServices;
#else
using System.ComponentModel;
#endif

namespace MyToolkit.MVVM
{
	[DataContract]
	public class NotifyPropertyChanged<TU> : NotifyPropertyChanged
	{
		public bool SetProperty<T>(Expression<Func<TU, T>> expression, ref T oldValue, T newValue)
		{
			return SetProperty(ref oldValue, newValue, ((MemberExpression)expression.Body).Member.Name);
		}

		public void RaisePropertyChanged<T>(Expression<Func<TU, T>> expression)
		{
			RaisePropertyChanged(((MemberExpression)expression.Body).Member.Name); 
		}
	}

	[DataContract]
    public class NotifyPropertyChanged : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public bool SetProperty<T>(String propertyName, ref T oldValue, T newValue)
		{
			if (object.Equals(oldValue, newValue))
				return false;

			oldValue = newValue;
			RaisePropertyChanged(propertyName);
			return true; 			
		}

#if METRO
		public bool SetProperty<T>(ref T oldValue, T newValue, [CallerMemberName] String propertyName = null)
		{
			return SetProperty(propertyName, ref oldValue, newValue); 
		} 

		public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
#else
		public void RaisePropertyChanged(string propertyName = null)
#endif
		{
			#if DEBUG
				#if METRO
					// TODO: search in subclasses
					//if (GetType().GetTypeInfo().GetDeclaredProperty(propertyName) == null)
					//	throw new ArgumentException("propertyName");
				#else
					if (GetType().GetProperty(propertyName) == null)
						throw new ArgumentException("propertyName");
				#endif
            #endif

			var copy = PropertyChanged; // avoid concurrent changes
			if (copy != null)
				copy(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}