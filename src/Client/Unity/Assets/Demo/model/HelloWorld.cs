using System;

[Serializable]
public class HelloWorld
{
    public string name;

    public HelloWorld(string name = "Nether")
    {
        this.name = name;
    }

}