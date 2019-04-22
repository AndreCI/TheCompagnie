using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface Subject
{
    void NotifyObservers();
    void AddObserver(Observer observer);

    void RemoveObserver(Observer observer);
}