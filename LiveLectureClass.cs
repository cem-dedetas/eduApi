using WebApplication1.Models;
namespace WebApplication1.Models

{
    public class LiveLectureClass
    {
        //has id, name, description, date, time, duration, lecturer, tags, attendees, channel name, recording link
        public int Id { get; set; }
        public string ?Description {get; set;}
        public DateTime Date {get; set;}
        public User Lecturer {get; set;} 
        public List<Tag> ? Tags {get; set;}
        public List<User> ? Attendees {get; set;}
        public string ChannelName { get; set; } = string.Empty;
        public string ? ShareUrl {get; set;} = string.Empty;
        public List<Recording> ? Recordings {get; set;}
        public List<ChatMessage>? ChatLog { get; set; }

    }

    public class LiveLectureDTO{
        public string Name { get; set; } = string.Empty;
        public string Description {get; set;} = string.Empty;
        public User ? Lecturer {get; set;}
        public List<Tag> ? Tags {get; set;}
        public string ChannelName {get; set;} = string.Empty;
    }

    public class someDTO
    {
        public string url { get; set; }
    }
}