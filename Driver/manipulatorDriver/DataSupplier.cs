using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace manipulatorDriver
{
    public abstract class DataSupplier
    {
        private List<Observer> observers;

        public DataSupplier()
        {
            observers = new List<Observer>();

        }

        public void Subscribe(Observer observer)
        {
            observers.Add(observer);

        }

        public void Unsubscribe(Observer observer)
        {
            observers.Remove(observer);

        }

        public void NotifyObservers(string data)
        {
            foreach(var observer in observers)
             {
                observer.getNotified(data);
            }
        }
    }
}
