using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct CellDirection
{
    public readonly Vector2Int Vector;

    CellDirection(int x, int y)
    {
        Vector = new Vector2Int(x, y);
    }

    public static readonly CellDirection None = new CellDirection(0, 0);
    public static readonly CellDirection North = new CellDirection(0, 1);
    public static readonly CellDirection South = new CellDirection(0, -1);
    public static readonly CellDirection East = new CellDirection(1, 0);
    public static readonly CellDirection West = new CellDirection(-1, 0);
    public static readonly CellDirection NorthEast = new CellDirection(1, 1);
    public static readonly CellDirection NorthWest = new CellDirection(-1, 1);
    public static readonly CellDirection SouthEast = new CellDirection(1, -1);
    public static readonly CellDirection SouthWest = new CellDirection(-1, -1);

    public static readonly List<CellDirection> CardinalDirections = new List<CellDirection>
    {
        North,
        East,
        South,
        West
    };

    public static readonly List<CellDirection> CardinalAndIntercardinalDirections = new List<CellDirection>
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    };

    public static bool operator ==(CellDirection a, CellDirection b)
    {
        return a.Vector == b.Vector;
    }

    public static bool operator !=(CellDirection a, CellDirection b)
    {
        return a.Vector != b.Vector;
    }

    public static explicit operator CellDirection(Vector2Int vector)
    {
        return CardinalAndIntercardinalDirections.DefaultIfEmpty(None).FirstOrDefault(direction => direction.Vector == vector);
    }

    public override bool Equals(object obj)
    {
        return obj is CellDirection ? this == (CellDirection)obj : false;
    }

    public override int GetHashCode()
    {
        return Vector.GetHashCode();
    }

    public override string ToString()
    {
        return Vector.ToString();
    }
}
