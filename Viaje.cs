
public class Viaje
{
    //  ATRIBUTOS
    private string codigo;
    private string origen;
    private string destino;
    private double distancia;
    private DateTime fechaProgramada;
    private DateTime fechaFin;
    private double carga;
    private Vehiculo vehiculo;
    private List<Chofer> choferes;

    // 2. CONSTRUCTOR 
    public Viaje(string codigo, string origen, string destino, double distancia, DateTime fechaProgramada, DateTime fechaFin, double carga, Vehiculo vehiculo, List<Chofer> choferes)
    {
        if (carga > vehiculo.CapacidadCarga)
        {
            throw new CapacidadExcedidaException("La carga a transportar supera la capacidad máxima del vehículo seleccionado.");
        }
        foreach (var chofer in choferes)
        {
            if (chofer.TieneSuperposicionHoraria(fechaHoraSalida, fechaHoraLlegada))
            {
                throw new ChoferOcupadoException($"El chofer {chofer.Nombre} ya se encuentra asignado a otro viaje en el mismo horario.");
            }
        
        this.codigo = codigo;
        this.origen = origen;
        this.destino = destino;
        this.distancia = distancia;
        this.fechaProgramada = fechaProgramada;
        this.fechaFin = fechaFin;
        this.carga = carga;
        this.vehiculo = vehiculo;
        this.choferes = choferes;

        // Registrar este viaje en la lista de cada chofer asignado
        foreach (var chofer in this.choferes)
        {
            chofer.Viajes.Add(this);
        }
    }

    // 3. MÉTODOS DE CÁLCULO Y GESTIÓN INTERNA
    public decimal CalcularCostoTotal()
    {
        double costoVehiculo = vehiculo.CalcularCostoOperativo(distanciaEstimada, cargaTotal);
        
        // Suma de sueldos básicos de los choferes
        double sueldoChoferes = 0;
        foreach (var chofer in choferes)
        {
            sueldoChoferes += chofer.SueldoBasico;
        }

        return costoVehiculo + sueldoChoferes;
    }

    
    public void GenerarInformeCSV()
    {
        string nombreArchivo = $"Informe_Viaje_{codigo}.csv";
        
        using (StreamWriter writer = new StreamWriter(nombreArchivo))
        {
            writer.WriteLine("--- INFORME DE VIAJE ---");
            writer.WriteLine($"Identificacion del Viaje,{codigo}");
            writer.WriteLine($"Ruta,{origen} - {destino}");
            writer.WriteLine($"Distancia ,{distancia} km");
            writer.WriteLine($"Vehiculo Utilizado,{vehiculo.matricula} ({vehiculo.codigoInterno})");
            writer.WriteLine($"Kilometraje Recorrido,{vehiculo.kilometraje} km");
            writer.WriteLine("--- CHOFERES ASIGNADOS ---");
            
            foreach (var chofer in choferes)
            {
                writer.WriteLine("Chofer,{chofer.nombre} (DNI: {chofer.dni})");
            }

            decimal costoVehiculo = vehiculo.CalcularCostoOperativo(distancia, carga);
            decimal costoChoferesTotal = 0;
            foreach (var chofer in choferes)
            {
                costoChoferesTotal += chofer.SueldoBasico;
            }

            writer.WriteLine("--- COSTOS DESGLOSADOS ---");
            writer.WriteLine($"Costo Operativo Vehiculo,{costoVehiculo}");
            writer.WriteLine($"Costo Total Sueldos Choferes,{costoChoferesTotal}");
            writer.WriteLine($"COSTO TOTAL DEL VIAJE,{CalcularCostoTotal()}");
        }
    }
}