using PulseLTD.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace PulseLTD.Models
{
    public class TaskDetail
    {
        public int Id { get; set; }

        [Required(ErrorMessage = MessageHelper.RequiredTitle)]
        [MaxLength(100,ErrorMessage = MessageHelper.MaxTitleLength)]
        public string Title { get; set; }

        [Display(Description ="Image")]
        [Required(ErrorMessage = MessageHelper.RequiredImage)]
        public string ImageName { get; set; }

        public string ImageText { get; set; }

        public DateTime DateConverted { get; set; }
    }
}