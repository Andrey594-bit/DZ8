using System;
using System.Collections.Generic;
namespace TumakovLab
{
     public class Song
    {
        private string name;
        private string author;
        private Song prev;

        public Song()
        {
            name = "";
            author = "";
            prev = null;
        }

        public Song(string name, string author)
        {
            this.name = name;
            this.author = author;
            this.prev = null;
        }

        public Song(string name, string author, Song prev)
        {
            this.name = name;
            this.author = author;
            this.prev = prev;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public void SetAuthor(string author)
        {
            this.author = author;
        }

        public void SetPrev(Song prev)
        {
            this.prev = prev;
        }

        public void Print()
        {
            Console.WriteLine($"{name} — {author}");
        }

        public string Title()
        {
            return $"{name} — {author}";
        }

        public override bool Equals(object d)
        {
            if (d is Song other)
                return this.name == other.name && this.author == other.author;
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(name, author);
        }
    }
}

   