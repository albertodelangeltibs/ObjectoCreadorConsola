using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Reflection;
using XmlToObjectPrueba01.ServiceReference1;
using System.Data;
using System.Text.RegularExpressions;

namespace XmlToObjectPrueba01
{
    class Program
    {
        static void Main(string[] args)
        {

            Serializer ser = new Serializer();
            string path = string.Empty;
            string xmlInputData = string.Empty;
            string xmlOutputData = string.Empty;

            path = Directory.GetCurrentDirectory() + @"\Salida.xml";
            xmlInputData = File.ReadAllText(path);

            Facturas factura = ser.Deserialize<Facturas>(xmlInputData);

            xmlOutputData = ser.Serialize<Facturas>(factura);

            List<PropiedadesFactura> listaFact = new List<PropiedadesFactura>();
            Concepto concepto = new Concepto();

            for (int i = 0; i < factura.Propiedades.Count; i++)
            {
                listaFact.Add(factura.Propiedades[i]);
            }

            IEnumerable<PropiedadesFactura> distint2 = listaFact.GroupBy(x => x.FolioABA).Select(y => y.First()).Where(element => element.FolioABA != "0").Distinct();

            IEnumerable<PropiedadesFactura> distint3 = listaFact.Select(
                    element => element).Where(
                    element => element.IdGeneral == 4);

            List<XmlToObjectPrueba01.ServiceReference1.Concepto> IdConceptos =
                new List<XmlToObjectPrueba01.ServiceReference1.Concepto>();

            foreach (PropiedadesFactura facturas in distint2)
            {
                Console.WriteLine(facturas.FolioABA);

                IEnumerable<PropiedadesFactura> distint4 = listaFact.Select(
                   element => element).Where(
                   element => element.IdGeneral == 4 && element.FolioABA == facturas.FolioABA);

                IEnumerable<PropiedadesFactura> distint5 = distint4.GroupBy(x => x.IdConcepto)
                    .Select(y => y.First()).Distinct();

                foreach(PropiedadesFactura createObject in distint5){

                    IEnumerable<PropiedadesFactura> objectCreated = listaFact
                        .Where(e => e.FolioABA == facturas.FolioABA &&
                        e.IdConcepto == createObject.IdConcepto);

                    XmlToObjectPrueba01.ServiceReference1.Concepto ConceptoClassWCF =
                    new XmlToObjectPrueba01.ServiceReference1.Concepto();

                    //object[,] prueba = new object[objectCreated.Count(), objectCreated.Count()];
                    //int i = 0;
                    //foreach (var Datos in objectCreated)
                    //{
                    //    prueba[i,0] = Datos.Valor;
                    //    prueba[0,i] = Datos.Dato;
                    //    i++;
                    //}
                    //i = 0;
                    var dictionary = new Dictionary<String, object>();
                    //int i = 0;
                    foreach (var Datos in objectCreated)
                    {
                        Regex regex = new Regex(@"\w*$");
                        Match match = regex.Match(Datos.Valor);
                        dictionary.Add(match.ToString(),Datos.Dato);
                        //Console.WriteLine(dictionary[i]);
                        //i++;
                    }
                    //i = 0;

                    PropertyInfo[] properties = typeof(XmlToObjectPrueba01.ServiceReference1.Concepto)
                        .GetProperties();
                    int x = 0;


                    foreach (PropertyInfo proper in properties)
                    {

                        if (proper.PropertyType == typeof(String) ||
                            proper.PropertyType == typeof(int))
                            
                        {
                            if (dictionary.ContainsKey(proper.Name.ToString()))
                            {
                                //object values = proper.Name;
                                //Console.WriteLine(dictionary[proper.Name.ToString()]);
                                proper.SetValue(ConceptoClassWCF, dictionary[proper.Name.ToString()]);
                            }
                        }else
                        if(proper.PropertyType == typeof(Decimal))
                        {
                            if (dictionary.ContainsKey(proper.Name.ToString()))
                            {
                                //object values = proper.Name;
                                //Console.WriteLine(dictionary[proper.Name.ToString()]);
                                proper.SetValue(ConceptoClassWCF, Convert.ToDecimal(dictionary[proper.Name.ToString()]));
                            }
                        }
                    }

                    IdConceptos.Add(ConceptoClassWCF);
                }
            }
            Console.ReadLine();
        }

        
    }
}
