using UnityEngine;
using System.Collections;

public interface ISelectable {

    void OnDefault();
    void OnHighlighted();
    void OnDehighlighted();
}
