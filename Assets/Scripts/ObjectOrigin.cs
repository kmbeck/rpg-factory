using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectOrigin<D> : EntityOrigin
where D : SOOrigin
{
    public D data;
}
