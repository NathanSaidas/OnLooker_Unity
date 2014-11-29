using System;
using UnityEngine;

[Serializable]
public class NativeObject 
{
    [SerializeField]
    private string m_Name;
    public virtual string name
    {
        get { return m_Name; }
        set { m_Name = value; }
    }
}
namespace EndevGame
{
    [Serializable]
    public class Object : IDisposable
    {

        private bool m_Disposed = false;

        [SerializeField]
        private string m_Name;
        public virtual string name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        //Invoke this when the object is being started in the game
        public virtual void start()
        {

        }
        //Invoke this when removing the object
        public virtual void destroy()
        {

        }
        //Gets called when disposing of the object.
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Use this method when disposing an object. To do the necessary cleanup
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if(!this.m_Disposed )
            {
                if(disposing)
                {
                    //Dispose Unmanaged Resources
                }

                m_Disposed = true;
            }
        }
    }
}