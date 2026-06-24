namespace FSM.Application.DTOs.WorkOrders;

public record CreateWorkOrderDto(
    string Title,
    string Description
);
//İçinde tarih veya ID yoktur, çünkü onları arka planda sistemin kendisi halleder.
//Dışarıdan sadece gerekli olan minimum bilgiyi almak için bu paketi kullanırız
//record kullanma sebebimiz değitirlemez olması ve Ram de az yer kaplaması