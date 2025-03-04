﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.DAL.Entities.News;

[Table("news", Schema = "news")]
[Index(nameof(URL), IsUnique = true)]
public class News
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [MaxLength(150)]
    required public string Title { get; set; }
    [Required]
    required public string Text { get; set; }
    [Required]
    [MaxLength(100)]
    required public string URL { get; set; }
    public int? ImageId { get; set; }
    public Image? Image { get; set; }
    [Required]
    public DateTime CreationDate { get; set; }
}
