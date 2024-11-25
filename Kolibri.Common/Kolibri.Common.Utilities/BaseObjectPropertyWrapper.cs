using System;
using System.ComponentModel;  

namespace Kolibri.Common.Utilities
{  

    /// <summary>  

    /// Defines a basic implementation of the <see cref="IObjectPropertyWrapper"/> interface.  

    /// </summary>  

    /// <typeparam name="TObject">  

    /// The type of the object that is beeing wrapped by this IObjectPropertyWrapper.  

    /// </typeparam>  

  public abstract class BaseObjectPropertyWrapper<TObject> : IObjectPropertyWrapper  

    {  

        #region [ Events ]  

   

        /// <summary>  

        /// Fired when any property of this IObjectPropertyWrapper has changed.  

        /// </summary>  

        /// <remarks>  

        /// Properties that wish to support binding, such as in Windows Presentation Foundation,  

        /// must notify the user that they have change.  

        /// </remarks>  

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;  

   

        #endregion  

   

        #region [Properties]  

   

        /// <summary>  

        /// Gets or sets the object this <see cref="IObjectPropertyWrapper"/> wraps around.  

        /// </summary>  

        /// <exception cref="ArgumentException">  

        /// If the Type of the given Object is not supported by this IObjectPropertyWrapper.  

        /// </exception>  

        [Browsable( false )]  

        public TObject WrappedObject  

        {  

            get { return _wrappedObject; }  

            set 

            {  

                if( value != null )  

                {  

                    if( !this.WrappedType.IsAssignableFrom( value.GetType() ) )  

                    {  

                        throw new ArgumentException(  

                            "The Type of the given Object is not supported by this IObjectPropertyWrapper.",  

                            "value" 

                        );  

                    }  

                }  

   

                _wrappedObject = value;  

                OnPropertyChanged( "WrappedObject" );  

            }  

        }  

   

        /// <summary>  

        /// Gets or sets the object this <see cref="IObjectPropertyWrapper"/> wraps around.  

        /// </summary>  

        [Browsable( false )]  

        object IObjectPropertyWrapper.WrappedObject  

        {  

            get { return this.WrappedObject;           }  

            set { this.WrappedObject = (TObject)value; }  

        }  

   

        /// <summary>  

        /// Receives the <see cref="Type"/> this <see cref="IObjectPropertyWrapper"/> wraps around.  

        /// </summary>  

        [Browsable( false )]  

        public Type WrappedType  

        {  

            get { return typeof( TObject ); }  

        }  

   

        #endregion  

   

        #region [ Methods ]  

   

        /// <summary>  

        /// Returns a clone of this <see cref="IObjectPropertyWrapper"/>.  

        /// </summary>  

        /// <remarks>  

        /// The wrapped object is not cloned, only the IObjectPropertyWrapper.  

        /// </remarks>  

        /// <returns>  

        /// The cloned IObjectPropertyWrapper.  

        /// </returns>  

        public abstract object Clone();  

   

        /// <summary>  

        /// Fires the <see cref="PropertyChanged"/> event.  

        /// </summary>  

        /// <param name="propertyName">  

        /// The name of the property whose value has changed.  

        /// </param>  

        protected void OnPropertyChanged( string propertyName )  

        {  

            if( PropertyChanged != null )  

                PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );  

        }  

   

        #endregion  

   

        #region [ Fields ]  

   

        /// <summary>  

        /// Stores the object this <see cref="IObjectPropertyWrapper"/> wraps around.  

        /// </summary>  

        private TObject _wrappedObject;  

   

        #endregion  

    }  

} 
