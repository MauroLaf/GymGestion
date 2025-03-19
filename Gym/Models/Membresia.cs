using System;
using System.Collections.Generic;

namespace Gym.Models;

public partial class Membresia
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Precio { get; set; }

    public DateTime FechaInicio { get; set; } //------------HACER MIGRACION SE AGREGO RECIEN PROPIEDAD PARA CALCULAR VENCIMIENTO
    public int DuracionDias { get; set; } //---SE ASIGNARA CON UN SERVICIO

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
