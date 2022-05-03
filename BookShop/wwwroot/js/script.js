let searchForm = document.querySelector('.search-form');
let loginForm = document.querySelector('.login-form-container');

document.querySelector('#search-btn').onclick = () => {
    searchForm.classList.toggle('active');
}

window.onscroll = () => {
    searchForm.classList.remove('active');

    if (window.scrollY > 80) {
        document.querySelector('.header .header-2').classList.add('active');
    } else {
        document.querySelector('.header .header-2').classList.remove('active');
    }

}

window.onload = () => {

    if (window.scrollY > 80) {
        document.querySelector('.header .header-2').classList.add('active');
    } else {
        document.querySelector('.header .header-2').classList.remove('active');
    }
    // fadeOut();
}

// function loader(){
//   document.querySelector('.loader-container').classList.add('active');
// }

// function fadeOut(){
//   setTimeout(loader, 4000);
// }

var swiper = new Swiper(".books-slider", {
    loop: true,
    centeredSlides: true,
    autoplay: {
        delay: 11500,
        disableOnInteraction: false,
    },
    breakpoints: {
        0: {
            slidesPerView: 1,
        },
        768: {
            slidesPerView: 2,
        },
        1024: {
            slidesPerView: 3,
        },
    },
});

var swiper = new Swiper(".featured-slider", {

    spaceBetween: 10,
    loop: true,
    centeredSlides: true,
    slidesPerView: 1,
    breakpoints: {
        640: {
            slidesPerView: 2,
        },
        992: {
            slidesPerView: 3,
        }
    },
    navigation: {
        nextEl: ".swiper-button-next",
        prevEl: ".swiper-button-prev",
    },
});

var swiper = new Swiper(".reviews-slider", {
    spaceBetween: 10,
    grabCursor: true,
    loop: true,
    centeredSlides: true,
    autoplay: {
        delay: 11500,
        disableOnInteraction: false,
    },
    breakpoints: {
        0: {
            slidesPerView: 1,
        },
        768: {
            slidesPerView: 2,
        },
        1024: {
            slidesPerView: 3,
        },
    },
});

var swiper = new Swiper(".helps-slider", {
    spaceBetween: 10,
    grabCursor: true,
    loop: true,
    centeredSlides: true,
    autoplay: {
        delay: 9500,
        disableOnInteraction: false,
    },
    breakpoints: {
        0: {
            slidesPerView: 1,
        },
        768: {
            slidesPerView: 2,
        },
        1024: {
            slidesPerView: 3,
        },
    },
});

function up(max) {
    document.getElementById("myNumber").value = parseInt(document.getElementById("myNumber").value) + 1;
    if (document.getElementById("myNumber").value >= parseInt(max)) {
        document.getElementById("myNumber").value = max;
    }
}
function down(min) {
    document.getElementById("myNumber").value = parseInt(document.getElementById("myNumber").value) - 1;
    if (document.getElementById("myNumber").value <= parseInt(min)) {
        document.getElementById("myNumber").value = min;
    }
}
let listCartItems = []; // coding convention
let total = 0;
const shoppingCart = document.querySelector('.shopping-cart');
const shoppingCartBtn = document.querySelector('.btn-cart');
const productItems = document.querySelector('.product-content-item');
const productImg = document.querySelector('.product-img');
let productdetails = document.querySelectorAll(".fa-eye");
let productdetail1 = document.querySelectorAll(".swiper-slide-arrivals");
let openproductdetails = document.querySelector('.product-detail');
let closeproductdetails = document.querySelector(".close-detail-btn");
let carts = document.querySelector('.fa-shopping-cart');  
let openpcarts = document.querySelector('.shop-cart');
let closecarts = document.querySelector('.close-cart-btn');
let cartItems = document.querySelector('.cart-item');
let totalbill = document.querySelector('.total-bill');

productdetails.forEach(item => item.addEventListener("click", viewproductdetail));
productdetail1.forEach(item => item.addEventListener("click", viewproductdetail));

function viewproductdetail( ) {
    let listDetailItems = [];
    productItems.innerHTML = '';
    productImg.innerHTML = '';
    openproductdetails.classList.add("open");

    const currentDetailBtn = event.target;
    const title = currentDetailBtn.dataset.title;
    console.log("title", title);
    const price = currentDetailBtn.dataset.price;
    console.log("title", price); 
    const description = currentDetailBtn.dataset.description;
    console.log("description", description);
    const category = currentDetailBtn.dataset.category;
    const url = currentDetailBtn.dataset.url;
    const isbn = currentDetailBtn.dataset.isbn;
    console.log("category", event.target);
    let item = { title, price, description, category, url, isbn };
    listDetailItems.push(item);

    listDetailItems.forEach(item => {
        let productDetailItem =  `               
                <p class = "product product-title">${item.title}</p>
                <p class="product product-category" data-category="${item.category}" data-id="123"> <span class = "title-bold">Category: </span> ${item.category} </p>
                <p class = "product product-description"> <span class = "title-bold">Description:</span> ${item.description} </p>
                <p class="product product-price">  ${item.price} </p>
                <a href="home/AddToCart/?Isbn=${item.isbn}" class="btn1 btn-cart>Add to cart</a>
            `;
        let productDetailItemImg =  `
                <img src="/img/${item.url}" alt="">               
            `;
    
        productItems.insertAdjacentHTML("afterbegin", productDetailItem);
        productImg.insertAdjacentHTML("afterbegin", productDetailItemImg);

    })
    listDetailItems.unshift(item);

}


/*shoppingCartBtn.addEventListener("click", function addToCart(event) {
    console.log(event.target.closest('.product-items').childNodes[3].childNodes[1].childNodes);
    console.log(event.target.closest('.product-items').getAttribute("class"));
    event.stopPropagation();
    const title = event.target.closest('.product-items').childNodes[3].childNodes[1].childNodes[1].innerText;
    console.log("title", title);
    const price = event.target.closest('.product-items').childNodes[3].childNodes[1].childNodes[7].innerText;
    console.log("title", price);
    const description = event.target.closest('.product-items').childNodes[3].childNodes[1].childNodes[5].innerText;
    console.log("description", description);
    const category = event.target.closest('.product-items').childNodes[3].childNodes[1].childNodes[3].innerText;
    console.log("category", event.target);
    const quantity = document.getElementById("myNumber").value;
    let itemcart = { title, price, description, category, quantity };
    listCartItems.push(itemcart);
    console.log('cart list item', listCartItems);

    openproductdetails.classList.remove("open");
    console.log('cart list item', listCartItems);

});
*/

closeproductdetails.addEventListener("click", closeDetail);

function closeDetail() {

    openproductdetails.classList.remove("open");
}

carts.addEventListener("click", openpcart);

function openpcart() {
    openpcarts.classList.add("open");

}

closecarts.addEventListener("click", closecart);

function closecart() {

    openpcarts.classList.remove("open");
}

