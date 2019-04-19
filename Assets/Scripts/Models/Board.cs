using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

public interface IBoard
{
}

public class Board : IBoard
{
    public class Factory : PlaceholderFactory<Board>
    {
    }
    
    public Board()
    {
    }
}