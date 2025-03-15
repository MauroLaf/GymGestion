using System;
using System.Collections.Generic;

namespace Gym.Models;

public partial class Clase
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Instructor { get; set; } = null!;

    public DateTime FechaHora { get; set; }

    public int CuposMax { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
