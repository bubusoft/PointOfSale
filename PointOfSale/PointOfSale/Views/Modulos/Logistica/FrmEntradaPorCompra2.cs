﻿using PointOfSale.Controllers;
using PointOfSale.Controllers.EvironmentController;
using PointOfSale.Models;
using PointOfSale.Views.Modulos.Busquedas;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PointOfSale.Views.Modulos.Logistica
{
    public partial class FrmEntradaPorCompra2 : Form
    {
        //objetos
        private Compra compra;
        private Comprap comprap;
        private Producto producto;
        private Proveedor proveedor;
        private CambiosPrecio cambiosPrecio;

        //listas
        private List<Comprap> partidas;
        private List<Impuesto> impuestos;
        public List<Lote> lotes { get; set; }

        //Controladores
        private CompraController compraController;
        private ComprapController comprapController;
        private LoteController loteController;
        private MovInvController movInvController;
        private FlujoController flujoController;
        private ProductoController productoController;
        private LaboratorioController laboratorioController;

        //Variables
        private int SigPartida;
        private const int NPARTIDAS = 400;
        private decimal subtotal;
        private decimal impuesto;

        public FrmEntradaPorCompra2()
        {
            InitializeComponent();
            ResetPDC();
        }


        #region Metodos
        private void ResetPDC()
        {
            //objetos
            compra = new Compra();
            comprap = new Comprap();
            producto = new Producto();
            proveedor = new Proveedor();
            cambiosPrecio = new CambiosPrecio();

            //listas
            partidas = new List<Comprap>();
            impuestos = new List<Impuesto>();
            lotes = new List<Lote>();

            //Controladores
            compraController = new CompraController();
            comprapController = new ComprapController();
            loteController = new LoteController();
            movInvController = new MovInvController();
            flujoController = new FlujoController();
            productoController = new ProductoController();
            laboratorioController = new LaboratorioController();
            //Variables
            SigPartida = 0;
            subtotal = 0;
            impuesto = 0;

            //Reset malla
            Malla.Rows.Clear();
            for (int i = 0; i < NPARTIDAS; i++)
            {
                Malla.Rows.Add();
                Malla.Rows[i].Cells[4].Style.BackColor = Color.Yellow;
                Malla.Rows[i].Cells[6].Style.BackColor = Color.Yellow;
                Malla.Rows[i].Cells[8].Style.BackColor = Color.Yellow;
                Malla.Rows[i].Cells[9].Style.BackColor = Color.Yellow;
                Malla.Rows[i].Cells[14].Style.BackColor = Color.Yellow;
                Malla.Rows[i].Cells[15].Style.BackColor = Color.Yellow;
            }

            CreaCompra();

        }
        private void CreaCompra()
        {
            compra = new Compra();
            compra.FechaDocumento = DpFechaDoc.Value;
            compra.FechaVencimiento = DpFechaVencimiento.Value;
            if (proveedor != null)
            {
                compra.ProveedorId = "SYS";
                compra.ProveedorName = "PROVEEDOR GENERICO";
            }

            compra.EsCxp = false;
            compra.CxpId = null;
            compra.Importe = 0;
            compra.Impuesto = 0;
            compra.FacturaProveedor = "";
            compra.TipoDocId = "COM";
            compra.EstadoDocId = "PEN";
            compra.AlmacenId = "1";
            compra.Datos = "SIN DATOS";
            compra.EstacionId = Ambiente.Estacion.EstacionId;
            compra.CreatedAt = DateTime.Now;
            compra.CreatedBy = Ambiente.LoggedUser.UsuarioId;


            compraController.InsertOne(compra);
        }
        private void LlenaDatosProducto()
        {

            TxtProductoId.Text = producto.ProductoId;
            TxtPrecioCompra.Text = producto.PrecioCompra.ToString();
            TxtPrecioCaja.Text = producto.PrecioCaja.ToString();
            TxtDescripcion.Text = producto.Descripcion;
            TxtU1.Text = producto.Utilidad1.ToString();
            TxtU2.Text = producto.Utilidad2.ToString();
            TxtU3.Text = producto.Utilidad3.ToString();
            TxtU4.Text = producto.Utilidad4.ToString();
            TxtPrecio1.Text = producto.Precio1.ToString();
            TxtPrecio2.Text = producto.Precio2.ToString();
            TxtPrecio3.Text = producto.Precio3.ToString();
            TxtPrecio4.Text = producto.Precio4.ToString();
            CargaListaImpuestos(producto);
            CargaGridImpuestos();
            TxtPrecioS1.Text = Ambiente.GetPrecioSalida(TxtPrecio1.Text, impuestos);
            TxtPrecioS2.Text = Ambiente.GetPrecioSalida(TxtPrecio2.Text, impuestos);
            TxtPrecioS3.Text = Ambiente.GetPrecioSalida(TxtPrecio3.Text, impuestos);
            TxtPrecioS4.Text = Ambiente.GetPrecioSalida(TxtPrecio4.Text, impuestos);
            PbxImagen.Image = GetImg(producto.RutaImg);
        }
        private void CargaGridImpuestos()
        {
            GridImpuestos.Rows.Clear();
            GridImpuestos.Refresh();
            foreach (var i in impuestos)
            {
                GridImpuestos.Rows.Add();
                GridImpuestos.Rows[GridImpuestos.RowCount - 1].Cells[0].Value = i.ImpuestoId;
                GridImpuestos.Rows[GridImpuestos.RowCount - 1].Cells[1].Value = i.Tasa;
            }
        }
        private void CargaListaImpuestos(Producto producto)
        {

            impuestos = new List<Impuesto>();

            ImpuestoController impuestoController = new ImpuestoController();

            if (!producto.Impuesto1Id.Equals("SYS"))
            {
                var impuesto1 = impuestoController.SelectOne(producto.Impuesto1Id);
                if (impuesto1 != null)
                    impuestos.Add(impuesto1);
            }

            if (!producto.Impuesto2Id.Equals("SYS"))
            {
                var impuesto2 = impuestoController.SelectOne(producto.Impuesto2Id);
                if (impuesto2 != null)
                    impuestos.Add(impuesto2);
            }
        }
        private Image GetImg(string ruta)
        {
            try
            {
                if (ruta == null)
                    return null;

                Image img = Image.FromFile(ruta);
                return img;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private void CalculaTotales()
        {
            subtotal = 0;
            impuesto = 0;
            foreach (var partida in partidas)
            {
                partida.Subtotal = partida.Cantidad * partida.PrecioCompra;
                partida.Subtotal -= partida.Subtotal * partida.Descuento;
                partida.ImporteImpuesto1 = partida.Subtotal * partida.Impuesto1;
                partida.ImporteImpuesto2 = partida.Subtotal * partida.Impuesto2;
                partida.Total = partida.Subtotal + partida.ImporteImpuesto1 + partida.ImporteImpuesto2;
                subtotal += partida.Subtotal;
                impuesto += partida.ImporteImpuesto1 + partida.ImporteImpuesto2;

                //reflejar totales
                TxtSubtotal.Text = Ambiente.FDinero(subtotal.ToString());
                TxtSubtotal.Text = Ambiente.FDinero((subtotal + impuesto).ToString());
            }
        }
        private void InsertaPartida()
        {
            if (producto == null && TxtProductoId.Text.Trim().Length == 0)
                Ambiente.Mensaje("Producto no encontrado");

            producto = productoController.SelectOne(TxtProductoId.Text.Trim());
            if (producto == null)
                return;

            if (compra.CompraId == 0)
            {
                Ambiente.Mensaje("La compra no existe");
                return;
            }

            //partida a la lista
            var partida = new Comprap();
            partida.CompraId = compra.CompraId;
            partida.ProductoId = producto.ProductoId;
            partida.Descripcion = producto.Descripcion;
            partida.LaboratorioId = producto.LaboratorioId;
            partida.LaboratorioName = laboratorioController.SelectOne(producto.LaboratorioId).Nombre.Trim();
            partida.Stock = producto.Stock;
            partida.Cantidad = NCantidad.Value;
            partida.PrecioCompra = Ambiente.ToDecimal(TxtPrecioCompra.Text);
            partida.PrecioCaja = Ambiente.ToDecimal(TxtPrecioCaja.Text);
            partida.Descuento = NDesc.Value / 100;
            partida.NImpuestos = impuestos.Count;

            partida.Impuesto1 = impuestos[0].Tasa;
            partida.Impuesto2 = impuestos[1].Tasa;
            partida.Subtotal = partida.Cantidad * partida.PrecioCompra;
            partida.Subtotal -= partida.Subtotal * partida.Descuento;
            partida.ImporteImpuesto1 = partida.Subtotal * partida.Impuesto1;
            partida.ImporteImpuesto2 = partida.Subtotal * partida.Impuesto2;
            partida.Total = partida.Subtotal + partida.ImporteImpuesto1 + partida.ImporteImpuesto2;
            partidas.Add(partida);

            CalculaTotales();
            // partida al grid
            Malla.Rows[SigPartida].Cells[0].Value = partida.ProductoId;
            Malla.Rows[SigPartida].Cells[1].Value = partida.Descripcion;
            Malla.Rows[SigPartida].Cells[2].Value = partida.LaboratorioName;
            Malla.Rows[SigPartida].Cells[3].Value = partida.Stock;
            Malla.Rows[SigPartida].Cells[4].Value = partida.Cantidad;
            Malla.Rows[SigPartida].Cells[5].Value = partida.PrecioCompra;
            Malla.Rows[SigPartida].Cells[6].Value = partida.Descuento;
            Malla.Rows[SigPartida].Cells[7].Value = partida.Impuesto1;
            Malla.Rows[SigPartida].Cells[8].Value = partida.Impuesto2;
            Malla.Rows[SigPartida].Cells[9].Value = partida.Subtotal;
            Malla.Rows[SigPartida].Cells[10].Value = partida.ImporteImpuesto1 + partida.ImporteImpuesto2;
            Malla.Rows[SigPartida].Cells[11].Value = partida.Subtotal + partida.ImporteImpuesto1 + partida.ImporteImpuesto2;
            Malla.Rows[SigPartida].Cells[12].Value = partida.NImpuestos;
            Malla.Rows[SigPartida].Cells[13].Value = ""; //pendiente
            Malla.Rows[SigPartida].Cells[14].Value = "";//pendiente
            ResetPartida();
        }
        private void LimpiarFilaMalla(int index)
        {
            Malla.Rows[index].Cells[0].Value = null;
            Malla.Rows[index].Cells[1].Value = null;
            Malla.Rows[index].Cells[2].Value = null;
            Malla.Rows[index].Cells[3].Value = null;
            Malla.Rows[index].Cells[4].Value = null;
            Malla.Rows[index].Cells[5].Value = null;
            Malla.Rows[index].Cells[6].Value = null;
            Malla.Rows[index].Cells[7].Value = null;
            Malla.Rows[index].Cells[8].Value = null;
            Malla.Rows[index].Cells[9].Value = null;
            Malla.Rows[index].Cells[10].Value = null;
            Malla.Rows[index].Cells[11].Value = null;
            Malla.Rows[index].Cells[12].Value = null;
            Malla.Rows[index].Cells[13].Value = null;
            Malla.Rows[index].Cells[14].Value = null;
        }
        private void Incrementa(int rowIndex)
        {
            if (partidas.Count > 0)
            {
                partidas[rowIndex].Cantidad++;
                CalculaTotales();
            }
        }
        private void Decrementa(int rowIndex)
        {
            if (partidas.Count > 0)
            {
                partidas[rowIndex].Cantidad--;
                CalculaTotales();
            }
        }
        private void ActualizaCantidad(decimal cant, int rowIndex)
        {
            if (partidas.Count > 0)
            {

            }
        }
        private void ResetPartida()
        {
            producto = null;
            TxtProductoId.Text = "";
            SigPartida++;
            TxtProductoId.Focus();
        }
        private void EliminaCompra()
        {
            if (compra != null)
            {
                compraController.DeletePartidas(compra);
                compraController.DeleteOne(compra.CompraId);
            }
        }

        private void EliminaPartida(int rowIndex, string descrip)
        {
            if (Ambiente.Pregunta("Realmente quiere borrar: " + descrip))
            {
                if (partidas.Count > 0 && rowIndex >= 0)
                {
                    partidas.RemoveAt(rowIndex);
                    SigPartida -= 1;
                    LimpiarFilaMalla(SigPartida);
                    CalculaTotales();
                }
            }

        }

        private void CerrarCompra(bool pendiente)
        {
            if (partidas.Count > 0)
            {
                compra.FechaDocumento = DpFechaDoc.Value;
                compra.FechaVencimiento = DpFechaVencimiento.Value;
                if (proveedor != null)
                {
                    compra.ProveedorId = proveedor.ProveedorId;
                    compra.ProveedorName = proveedor.RazonSocial.Trim().Length == 0 ? proveedor.Negocio.Trim() : proveedor.RazonSocial.Trim();
                }
                else
                {
                    compra.ProveedorId = "SYS";
                    compra.ProveedorName = "PROVEEDOR GENERICO";

                }
                compra.EsCxp = false;
                compra.CxpId = null;
                compra.Importe = subtotal;
                compra.Impuesto = impuesto;
                compra.FacturaProveedor = TxtFacturaProveedor.Text.Trim().Length == 0 ? "SYS" : TxtFacturaProveedor.Text.Trim();
                compra.TipoDocId = "COM";

                if (!pendiente)
                    compra.EstadoDocId = "CON";
                else
                    compra.EstadoDocId = "PEN";

                compra.AlmacenId = "1";
                compra.Datos = TxtDatosProveedor.Text.Trim();
                if (compraController.Update(compra))
                {
                    if (GuardaPartidas() && !pendiente)
                        Reportes.EntradaXCompra(compra.CompraId);
                    else
                        Ambiente.Mensaje("Algo salió mal con: GuardaPartidas()");
                }
                else
                    Ambiente.Mensaje("Algo salió mal con: compraController.Update(compra)");
            }
            else
                Ambiente.Mensaje("Sin productos.");
        }
        private void AfectaMovsInv()
        {
            foreach (var p in partidas)
            {
                //var movInv = new MovInv();
                //movInv.ConceptoMovsInvId = "VEN";
                //movInv.NoRef = (int)venta.NoRef;
                //movInv.EntradaSalida = "S";
                //movInv.IdEntrada = null;
                //movInv.IdSalida = p.VentapId;
                //movInv.ProductoId = p.ProductoId;
                //movInv.Precio = p.Precio;
                //movInv.Cantidad = p.Cantidad;
                //movInv.CreatedAt = DateTime.Now;
                //movInv.CreatedBy = Ambiente.LoggedUser.UsuarioId;
                //movInvController.InsertOne(movInv);
            }
        }

        private void PendienteOdescarta()
        {
            if (partidas.Count > 0)
            {
                if (Ambiente.Pregunta("Quiere dejar la venta como pendiente"))
                    CerrarCompra(true);
                else
                    EliminaCompra();
            }
            else
                EliminaCompra();
            Close();
        }

        private bool GuardaPartidas()
        {
            return comprapController.InsertRange(partidas);
        }


        #endregion




        #region Eventos
        private void TxtProvedorId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                using (var form = new FrmBusqueda(TxtProvedorId.Text, (int)Ambiente.TipoBusqueda.Proveedores))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        proveedor = form.Proveedor;
                        TxtProvedorId.Text = proveedor.ProveedorId;
                        TxtDatosProveedor.Text = proveedor.Negocio + " " + proveedor.Direccion + " " + proveedor.Colonia
                            + " " + proveedor.Municipio + " " + proveedor.Localidad + " " + proveedor.Estado + " TEL." + proveedor.Telefono;
                    }
                }
            }
        }

        private void TxtProductoId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                producto = productoController.SelectOne(TxtProductoId.Text);
                if (producto != null)
                {
                    LlenaDatosProducto();
                    NCantidad.Focus();
                    return;
                }
                using (var form = new FrmBusqueda(TxtProductoId.Text, (int)Ambiente.TipoBusqueda.Productos))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        TxtProductoId.Text = form.Producto.ProductoId;
                    }
                }
            }
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            InsertaPartida();
        }

        private void TxtU1_Leave(object sender, EventArgs e)
        {
            TxtU1.Text = Ambiente.FDinero(TxtU1.Text);
            TxtPrecio1.Text = Ambiente.GetPrecio(TxtPrecioCompra.Text, TxtU1.Text);
        }

        private void TxtPrecio1_Leave(object sender, EventArgs e)
        {
            TxtPrecio1.Text = Ambiente.FDinero(TxtPrecio1.Text);
            TxtU1.Text = Ambiente.GetMargen(TxtPrecioCompra.Text, TxtPrecio1.Text);
            TxtPrecioS1.Text = Ambiente.GetPrecioSalida(TxtPrecio1.Text, impuestos);
        }

        private void TxtU2_Leave(object sender, EventArgs e)
        {
            TxtU2.Text = Ambiente.FDinero(TxtU2.Text);
            TxtPrecio2.Text = Ambiente.GetPrecio(TxtPrecioCompra.Text, TxtU2.Text);
        }

        private void TxtPrecio2_Leave(object sender, EventArgs e)
        {
            TxtPrecio2.Text = Ambiente.FDinero(TxtPrecio2.Text);
            TxtU2.Text = Ambiente.GetMargen(TxtPrecioCompra.Text, TxtPrecio2.Text);
            TxtPrecioS2.Text = Ambiente.GetPrecioSalida(TxtPrecio2.Text, impuestos);
        }

        private void TxtU3_Leave(object sender, EventArgs e)
        {
            TxtU3.Text = Ambiente.FDinero(TxtU3.Text);
            TxtPrecio3.Text = Ambiente.GetPrecio(TxtPrecioCompra.Text, TxtU3.Text);
        }

        private void TxtPrecio3_Leave(object sender, EventArgs e)
        {
            TxtPrecio3.Text = Ambiente.FDinero(TxtPrecio3.Text);
            TxtU3.Text = Ambiente.GetMargen(TxtPrecioCompra.Text, TxtPrecio3.Text);
            TxtPrecioS3.Text = Ambiente.GetPrecioSalida(TxtPrecio3.Text, impuestos);
        }

        private void TxtU4_Leave(object sender, EventArgs e)
        {
            TxtU4.Text = Ambiente.FDinero(TxtU4.Text);
            TxtPrecio4.Text = Ambiente.GetPrecio(TxtPrecioCompra.Text, TxtU4.Text);
        }

        private void TxtPrecio4_Leave(object sender, EventArgs e)
        {
            TxtPrecio4.Text = Ambiente.FDinero(TxtPrecio4.Text);
            TxtU4.Text = Ambiente.GetMargen(TxtPrecioCompra.Text, TxtPrecio4.Text);
            TxtPrecioS4.Text = Ambiente.GetPrecioSalida(TxtPrecio4.Text, impuestos);
            BtnAgregar.Focus();
        }

        private void BtnAceptar_Click(object sender, EventArgs e)
        {
            CerrarCompra(false);
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            PendienteOdescarta();
        }


        private void TxtProductoId_Leave(object sender, EventArgs e)
        {
            if (TxtProductoId.Text.Trim().Length > 0)
            {
                producto = productoController.SelectOne(TxtProductoId.Text);
                if (producto != null)
                {
                    LlenaDatosProducto();
                }
            }
        }

        private void Malla_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            ActualizaCantidad(decimal.Parse(Malla.CurrentCell.Value.ToString()), e.RowIndex);
            CalculaTotales();
            TxtProductoId.Focus();
        }

        private void Malla_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(ColumnCant_KeyPress);
            if (Malla.CurrentCell.ColumnIndex == 3) //Desired Column
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(ColumnCant_KeyPress);
                }
            }
        }
        private void ColumnCant_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        #endregion

        private void Malla_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Oemplus)
                Incrementa(Malla.CurrentCell.RowIndex);
            else if (e.KeyCode == Keys.OemMinus)
                Decrementa(Malla.CurrentCell.RowIndex);
            else if (e.KeyCode == Keys.Delete)
            {
                if (Malla.Rows[Malla.CurrentCell.RowIndex].Cells[3].Value != null)
                    EliminaPartida(Malla.CurrentCell.RowIndex, Malla.Rows[Malla.CurrentCell.RowIndex].Cells[0].Value.ToString());
            }
        }
    }
}