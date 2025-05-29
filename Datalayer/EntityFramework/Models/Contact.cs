using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.EntityFramework.Models;
public class Contact: BaseEntity
{
	public string full_name { get; set; }
	public string mobile { get; set; }
	public string message { get; set; }
}

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
	public void Configure(EntityTypeBuilder<Contact> builder)
	{
		builder.HasKey(x => x.id);
		builder.Property(x => x.full_name).IsRequired();
		builder.Property(x => x.mobile).IsRequired();
		builder.Property(x => x.message).IsRequired();
		builder.Property(x => x.slug).IsRequired();
		builder.HasIndex(x => x.slug).IsUnique();
	}
}