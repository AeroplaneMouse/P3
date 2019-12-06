using AMS.Models;

namespace AMS.Events
{
    public delegate void NotificationEventHandler(Notification n);

    public delegate void SqlConnectionEventHandler();

    public delegate void PromptEventHandler(object sender, PromptEventArgs e);

    public delegate void ExpandedPromptEventHandler(object sender, ExpandedPromptEventArgs e);

    public delegate void TextInputPromptEventHandler(object sender, TextInputPromptEventArgs e);

    public delegate void FieldInputPromptEventHandler(object sender, FieldInputPromptEventArgs e);
}
