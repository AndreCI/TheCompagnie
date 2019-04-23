using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface Subject
{
    void NotifyObservers(GeneralUtils.SUBJECT_TRIGGER trigger);
    void AddObserver(Observer observer, GeneralUtils.SUBJECT_TRIGGER trigger);

    void RemoveObserver(Observer observer, GeneralUtils.SUBJECT_TRIGGER trigger);
}