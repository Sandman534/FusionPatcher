using System;

namespace Fusion
{
        public class Rootobject
    {
        public Prelude? prelude { get; set; }
        public Common1[]? common { get; set; }
        public string[]? bash_tags { get; set; }
        public Global[]? globals { get; set; }
        public Group[]? groups { get; set; }
        public Plugin[]? plugins { get; set; }
    }

    public class Prelude
    {
        public Common[]? common { get; set; }
    }

    public class Common
    {
        public string? type { get; set; }
        public Content[]? content { get; set; }
        public string[]? subs { get; set; }
        public string? condition { get; set; }
        public string? util { get; set; }
        public Info[]? info { get; set; }
    }

    public class Content
    {
        public string? lang { get; set; }
        public string? text { get; set; }
    }

    public class Info
    {
        public string? lang { get; set; }
        public string? text { get; set; }
    }

    public class Common1
    {
        public string? name { get; set; }
        public string? display { get; set; }
        public string? condition { get; set; }
        public string? type { get; set; }
        public Content1[]? content { get; set; }
        public string[]? subs { get; set; }
    }

    public class Content1
    {
        public string? lang { get; set; }
        public string? text { get; set; }
    }

    public class Global
    {
        public string? type { get; set; }
        public Content2[]? content { get; set; }
        public string[]? subs { get; set; }
        public string? condition { get; set; }
    }

    public class Content2
    {
        public string? lang { get; set; }
        public string? text { get; set; }
    }

    public class Group
    {
        public string? name { get; set; }
        public string[]? after { get; set; }
        public string? description { get; set; }
    }

    public class Plugin
    {
        public string? name { get; set; }
        public string? group { get; set; }
        public object[]? tag { get; set; }
        public Dirty[]? dirty { get; set; }
        public Msg[]? msg { get; set; }
        public object[]? after { get; set; }
        public Clean[]? clean { get; set; }
        public object[]? url { get; set; }
        public object[]? inc { get; set; }
        public object[]? req { get; set; }
    }

    public class Dirty
    {
        public string? util { get; set; }
        public Info1[]? info { get; set; }
        public string? crc { get; set; }
        public string? itm { get; set; }
        public string? udr { get; set; }
        public string? nav { get; set; }
    }

    public class Info1
    {
        public string? lang { get; set; }
        public string? text { get; set; }
    }

    public class Msg
    {
        public string? type { get; set; }
        public Content3[]? content { get; set; }
        public string[]? subs { get; set; }
        public string? condition { get; set; }
    }

    public class Content3
    {
        public string? lang { get; set; }
        public string? text { get; set; }
    }

    public class Clean
    {
        public string? crc { get; set; }
        public string? util { get; set; }
    }
}
