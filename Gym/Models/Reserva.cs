using System;
using System.Collections.Generic;

namespace Gym.Models;

public partial class Reserva
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public int ClaseId { get; set; }

    public DateTime FechaReserva { get; set; }

    public virtual Clase Clase { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
