using ResumeApp.Classes;
using ResumeApp.ViewModels;
using System;
using System.Collections.Generic;
using TweetSharp;
using Xamarin.Forms.Xaml;

namespace ResumeApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TwitterPage : BaseContentPage
    {
        private TwitterService service;
        private string Key = "gZtCkMEqvD90SNvKiVlHHcF0q";
        private string Secret = "x2QurQBvZplmfGMUvAMG9NRJwwhAv0ouFPF9iUss2BtENOxp5d";
        private string AccessToken = "885668199492182016-lV6K08gWhqJmFMs3IpykvDsP9af0Z1n";
        private string AccessTokenSecret = "S5j3Rk9PE2tBVhEQjWRABQl6uxqZVfNP3jJ3O5D7q68rw";
        private List<TwitterStatus> TweetsToDelete = new List<TwitterStatus>();
        private TwitterViewModel VM;

        public TwitterStatus SendTweet(string message)
        {
            return service.SendTweet(
                new SendTweetOptions
                {
                    Status = message
                });
        }

        public TwitterStatus ReplyToTweet(string message, long ReplyID)
        {
            return service.SendTweet(
                new SendTweetOptions()
                {
                    Status = message,
                    InReplyToStatusId = ReplyID
                });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            for (int x = TweetsToDelete.Count - 1; x >= 0; x--)
            {
                try
                {
                    DeleteTweet(TweetsToDelete[x].Id);
                }
                catch
                {// tweets can be returned null
                }
            }
        }

        public IEnumerable<TwitterStatus> GetTweetsFromHome(int Count = 20, bool? ExcludeReplies = null, long? SinceID = null)
        {
            return service.ListTweetsOnHomeTimeline(
                new ListTweetsOnHomeTimelineOptions()
                {
                    Count = Count,
                    ExcludeReplies = ExcludeReplies,
                    SinceId = SinceID
                });
        }

        public IEnumerable<TwitterStatus> GetRepliesTo(long ID, int Count = 20, int CountToLookThrough = 100)
        {
            List<TwitterStatus> Replies = new List<TwitterStatus>();
            try
            {
                IEnumerable<TwitterStatus> AllTweetsSince = service.ListTweetsOnUserTimeline(new ListTweetsOnUserTimelineOptions() { SinceId = ID, Count = CountToLookThrough });
                foreach (TwitterStatus Tweet in AllTweetsSince)
                {
                    if (Replies.Count > Count)
                        break;
                    if (Tweet.InReplyToStatusId == ID)
                        Replies.Add(Tweet);
                }
            }
            catch
            {
            }
            return Replies;
        }

        private IEnumerable<TwitterStatus> tweets;

        public TwitterPage()
        {
            InitializeComponent();
            VM = BindingContext as TwitterViewModel;
            service = new TwitterService(Key, Secret, AccessToken, AccessTokenSecret);
            TwitterStatus CurrentTweet = SendTweet("Hello World");
            TweetsToDelete.Add(CurrentTweet);
            tweets = GetTweetsFromHome();
            foreach (TwitterStatus tweet in tweets)
                VM.TweetsPickerSource.Add(tweet.Text);
        }

        public TwitterStatus DeleteTweet(long ID)
        {
            return service.DeleteTweet(new DeleteTweetOptions() { Id = ID });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void NewTweetSendButton_Clicked(object sender, EventArgs e)
        {
            if (NewTweetTextEditor.Text != "")
            {
                TweetsToDelete.Add(SendTweet(NewTweetTextEditor.Text));
                tweets = GetTweetsFromHome();
                foreach (TwitterStatus tweet in tweets)
                    VM.TweetsPickerSource.Add(tweet.Text);
            }
        }

        private void ReplyTweetSendButton_Clicked(object sender, EventArgs e)
        {
            if (ReplyTweetTextEditor.Text != "")
            {
                long id = 0;
                foreach (TwitterStatus tweet in tweets)
                {
                    if (tweet.Text == TweetsPicker.SelectedItem as string)
                        id = tweet.Id;
                }
                TweetsToDelete.Add(ReplyToTweet(ReplyTweetTextEditor.Text, id));
                tweets = GetTweetsFromHome();
                foreach (TwitterStatus tweet in tweets)
                    VM.TweetsPickerSource.Add(tweet.Text);
            }
        }
    }
}