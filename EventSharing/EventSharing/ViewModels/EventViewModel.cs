using EventSharing.Models;
using System.ComponentModel.DataAnnotations;

namespace EventSharing.ViewModels
{
    //ViewModel: c'est une classe qui contient les données que nous voulons afficher
    //dans la vue, elle peut contenir des propriétés de plusieurs modèles différents,
    //elle permet de faire le lien entre les modèles et la vue,
    //elle permet de ne pas exposer directement les modèles à la vue,
    //elle permet de faire des calculs ou des transformations sur les données
    //avant de les afficher dans la vue.
    public class EventViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="le nom est obligatoire")]
        [StringLength(100,ErrorMessage ="le nom ne peut pas dépasser 100 caractères")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "la description est obligatoire")]
        [StringLength(500, ErrorMessage = "la description ne peut pas dépasser 500 caractères")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "la date de début est obligatoire")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "la date de fin est obligatoire")]

        public DateTime? EndDate { get; set; }

        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<CategoryViewModel>? CategoriesVm { get; set; }
    }
}
