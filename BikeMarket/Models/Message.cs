using System;
using System.Collections.Generic;

namespace BikeMarket.Models;

public partial class Message
{
    public int Id { get; set; }

    public int ConversationId { get; set; }

    public int SenderId { get; set; }

    public string? Content { get; set; }

    public string MessageType { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? DeliveredAt { get; set; }

    public DateTime? SeenAt { get; set; }

    public string? AttachmentUrl { get; set; }

    public string? AttachmentName { get; set; }

    public long? AttachmentSize { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime SentAt { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual User Sender { get; set; } = null!;
}
