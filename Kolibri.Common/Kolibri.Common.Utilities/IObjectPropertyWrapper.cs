using System;
using System.ComponentModel;  

namespace Kolibri.Common.Utilities
 {  

     /// <summary>  

     /// An <see cref="IObjectPropertyWrapper"/> is responsible  

     /// for exposing the properties of an Object which the user is intended to edit.  

     /// </summary>  

  public interface IObjectPropertyWrapper : ICloneable, INotifyPropertyChanged  

     {  

         /// <summary>  

         /// Gets or sets the object this <see cref="IObjectPropertyWrapper"/> wraps around.  

         /// </summary>  

         [Browsable( false )]  

         object WrappedObject { get; set; }  

    

         /// <summary>  

         /// Receives the <see cref="Type"/> this <see cref="IObjectPropertyWrapper"/> wraps around.  

         /// </summary>  

         [Browsable( false )]  

         Type WrappedType { get; }  

     }  

    

 } 
