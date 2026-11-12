
public class Viaje
{
    //  ATRIBUTOS
    private string codigo;
    private string origen;
    private string destino;
    private double distancia;
    public DateTime FechaProgramada { get; set; }
    public DateTime FechaFin { get; set; }
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
            if (!chofer.disponible(fechaProgramada, fechaFin))
       
            {
                throw new ChoferOcupadoException($"El chofer {chofer.Nombre} ya se encuentra asignado a otro viaje en el mismo horario.");
            }
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
            chofer.listaViajes.Add(this);
        }
    }

    // 3. MÉTODOS DE CÁLCULO Y GESTIÓN INTERNA
     public double CalcularCostoVehiculo()
        {
            return (double)vehiculo.CalcularCostoOperativo(carga, distancia);
        }
     public double CalcularCostoSueldosChoferes()
        {
            double sueldoChoferes = 0;
            foreach (var chofer in choferes)
            {
                sueldoChoferes += (double)chofer.ObtenerSueldoBasico();
            }
            return sueldoChoferes;
        }

    public double CalcularCostoTotal()
        {
            double costoVehiculo = CalcularCostoVehiculo();
            double sueldoChoferes = CalcularCostoSueldosChoferes();
            return costoVehiculo + sueldoChoferes;
        }
    
    public void GenerarInformeCSV()
    {
        string path = $"Informe_Viaje_{codigo}.csv";

        using (StreamWriter sw = new StreamWriter(path))
        {
            sw.WriteLine("IdentificacionViaje;Vehiculo;Choferes;KilometrajeRecorrido;CostoOperativoVehiculo;CostoSueldosChoferes;CostoTotal");

            
            string nombresChoferes = string.Join(" / ", choferes.Select(c => c.Nombre));

           
            sw.WriteLine("{0};{1};{2};{3} km;${4};${5};${6}",
                codigo,
                vehiculo.Matricula,
                nombresChoferes,
                distancia,
                CalcularCostoVehiculo(),
                CalcularCostoSueldosChoferes(),
                CalcularCostoTotal()
            );
        }

        Console.WriteLine($"¡Verificación exitosa! Se generó el archivo: {path}");
    }