using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Ranking : IComparable<Ranking>
{
    //Las 3 propiedades que tendrá el ranking
    //la fecha no la usamos en ningun momento así que la quitamos
    public int Id { get; set; }
    public string Name { get; set; }
    public int Score { get; set; }

    public Ranking(int id, string name, int score)
    {
        this.Id = id;
        this.Name = name;
        this.Score = score;
    }

    public int CompareTo(Ranking other)
    {      
        //el que recibe > que el que tiene = -1
        //el que recibe < que el que tiene = 1
        //0
        if (other.Score > this.Score)
        {
            return 1;
        }
        else if (other.Score < this.Score)
        {
            return -1;
        }        
        return 0;
    }
}
