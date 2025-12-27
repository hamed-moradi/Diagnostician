using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Model.Binding {
  public class SupplierSaleLogBindingModel {
    [Required(ErrorMessage = $"{nameof(DataOwnerKey)} is requiered!")]
    public Guid DataOwnerKey { get; set; }
    [Required(ErrorMessage = $"{nameof(DataOwnerCenterKey)} is requiered!")]
    public Guid DataOwnerCenterKey { get; set; }
    [Required(ErrorMessage = $"{nameof(DealerUniqueId)} is requiered!")]
    public Guid DealerUniqueId { get; set; }
    [Required(ErrorMessage = $"{nameof(TourId)} is requiered!")]
    public Guid TourId { get; set; }
    [Required(ErrorMessage = $"{nameof(Order)} is requiered!")]
    public BarookOrderResponseModel Order { get; set; }
  }

  public class BarookRegisterOrderItemModel {
    public string Name { get; set; }
    public double SaleCount { get; set; }
    public double Amount { get; set; }
    public string UnitType { get; set; }
  }

  public class BarookOrderResponseModel {
    public string ShopNationalCode { get; set; }
    public string ShopName { get; set; }
    public string SaleStatus { get; set; }
    public double TotalAmount { get; set; }
    public string ShopFirstName { get; set; }
    public string ShopLastName { get; set; }
    public string ExternalCode { get; set; }
    public List<BarookRegisterOrderItemModel> SaleItemDto { get; set; }
    public string Code { get; set; }
    public string ShopMobile { get; set; }
    public int PaymentMonthCount { get; set; }
    public string ShopTelephoneNumber { get; set; }
    public string ShopAddress { get; set; }
    public string CreatedAt { get; set; }
    public string CreatedDate { get; set; }
    public string PaymentDate { get; set; }
    public string TransactionDate { get; set; }
    public string OwnerName { get; set; }
    public string OwnerMobile { get; set; }
    public string OwnerNationalCode { get; set; }
    public string CardHolderName { get; set; }
    public string TerminalSerial { get; set; }
    public string TerminalNumber { get; set; }
    public string BrokerNumber { get; set; }
    public Guid UniqueId { get; set; }
  }
}
