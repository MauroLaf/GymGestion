using System;
using System.Collections.Generic;

namespace Gym.Models;

public partial class Membresia
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Precio { get; set; }

    public int DuracionDias { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
