
$(function () {
    $("#ShopCar").mouseover(shopCar_mouseover);
    $("#ShopCar").mouseout(shopCar_mouseout);
    $(".search-btn").mouseover(Search_btnImg);
    $(".search-btn").mouseout(Search_btnImg_out);

    $(".left_up").mouseover(left_up_over);
    $(".left_up").mouseout(left_up_out);

    $(".right_next").mouseover(right_next_over);
    $(".right_next").mouseout(right_next_out);

    
});

function shopCar_mouseover() {
    $("#ShopCar").css("background", "#fff");
    $("#ShopCar a").css("color", "#ff6700");
}

function shopCar_mouseout() {
    $("#ShopCar").css("background", "#424242");
    $("#ShopCar a").css("color", "#b0b0b0");
}

function Search_btnImg() {
    $(".search-btn").css("background-image", "url(/Images/red-search.png)");
}

function Search_btnImg_out() {
    $(".search-btn").css("background-image", "url(/Images/search.png)");
}

function left_up_over() {
    $(".left_up").css("opacity", 0.9);
}

function left_up_out() {
    $(".left_up").css("opacity", 0.1);
}

function right_next_over() {
    $(".right_next").css("opacity", 0.9);
}

function right_next_out() {
    $(".right_next").css("opacity", 0.1);
}

