using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace _4square.Models
{
	public class Reference
	{
		[Key]
		public int ID { get; set; }
		public string Text { get; set; }
		public int References { get; set; }
	}

	public class ReferenceCounter
	{
		[Required(ErrorMessage = "Please enter some place")]
		[Display(Name = "Place in Brno: ")]
		public string placeToSearch { get; set; }

		[Display(Name = "Use my location: ")]
		public bool userLocation { get; set; }
	}

	public class ReferencesDBContext : DbContext
	{
			public DbSet<Reference> References { get; set; }
	}
}
