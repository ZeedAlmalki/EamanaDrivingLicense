const API_BASE_URL = 'http://localhost:5281/api';

// دالة التنقل بين الصفحات مع التحقق من حالة تسجيل الدخول للخدمات
function showPage(pageId) {
    const currentUser = JSON.parse(localStorage.getItem('currentUser'));
    
    // حماية الصفحات التي تتطلب تسجيل دخول مسبق
    if ((pageId === 'new-license' || pageId === 'query' || pageId === 'details') && !currentUser) {
        alert('الرجاء تسجيل الدخول أولاً للوصول إلى هذه الخدمات.');
        showPage('login');
        return;
    }

    const pages = document.querySelectorAll('.page');
    pages.forEach(page => page.classList.remove('active'));
    
    const targetPage = document.getElementById(pageId);
    if(targetPage) {
        targetPage.classList.add('active');
        window.scrollTo(0, 0);

        // إذا انتقل لصفحة الاستعلام، يتم جلب طلبات هذا المستخدم تلقائياً
        if (pageId === 'query' && currentUser) {
            loadUserApplications(currentUser.NationalID || currentUser.nationalID);
        }
    }
}

// تحديث الهيدر ديناميكياً (عرض رقم الهوية وزر تسجيل الخروج)
function updateHeaderUI() {
    const user = JSON.parse(localStorage.getItem('currentUser'));
    const navLeft = document.getElementById('navLeft');
    if (!navLeft) return;

    if (user) {
        const nationalId = user.NationalID || user.nationalID || '';
        navLeft.innerHTML = `
            <span style="color: #333; font-size: 13px; margin-left: 12px; font-weight: bold;">
                مسجل الدخول برقم هوية: ${nationalId}
            </span>
            <a onclick="logout()" style="color: #c0392b; cursor: pointer; font-weight: bold;">تسجيل الخروج</a>
        `;
    } else {
        navLeft.innerHTML = `
            <a onclick="showPage('register')">إنشاء حساب</a>
            <a onclick="showPage('login')">تسجيل الدخول</a>
        `;
    }
}

// تسجيل الخروج
function logout() {
    localStorage.removeItem('currentUser');
    updateHeaderUI();
    showPage('home');
    alert('تم تسجيل الخروج بنجاح.');
}

// جلب الدول وفئات الرخص
async function loadCountries() {
    try {
        const response = await fetch(`${API_BASE_URL}/countries/all`);
        const countries = await response.json();
        const countrySelect = document.getElementById('countrySelect');
        if (countrySelect) {
            countrySelect.innerHTML = '<option value="">الرجاء الاختيار</option>';
            countries.forEach(country => {
                const option = document.createElement('option');
                option.value = country.CountryID;
                option.textContent = country.CountryName;
                countrySelect.appendChild(option);
            });
        }
    } catch (error) { console.error('خطأ في جلب الدول:', error); }
}

async function loadLicenseClasses() {
    try {
        const response = await fetch(`${API_BASE_URL}/license-classes/all`);
        const classes = await response.json();
        const licenseSelect = document.getElementById('licenseClassSelect');
        if (licenseSelect) {
            licenseSelect.innerHTML = '<option value="">الرجاء الاختيار</option>';
            classes.forEach(cls => {
                const option = document.createElement('option');
                option.value = cls.LicenseClassID;
                option.textContent = cls.ClassName;
                licenseSelect.appendChild(option);
            });
        }
    } catch (error) { console.error('خطأ في جلب فئات الرخص:', error); }
}

// جلب طلبات رخص القيادة الخاصة بالمستخدم الحالي فقط وعرضها في الجدول
async function loadUserApplications(nationalID) {
    try {
        
        const response = await fetch(`${API_BASE_URL}/orders/national/${nationalID}`);
        const tbody = document.querySelector('.data-table tbody');
        
        if (response.ok && tbody) {
            const apps = await response.json();
            tbody.innerHTML = '';
            
            if (!apps || apps.length === 0) {
                tbody.innerHTML = '<tr><td colspan="4" style="text-align: center; padding: 15px;">لا توجد طلبات رخص مسجلة باسمك حالياً.</td></tr>';
                return;
            }

            apps.forEach(app => {
                tbody.innerHTML += `
                    <tr onclick="showPage('details')">
                        <td>${app.OrderID || app.orderID}</td>
                        <td>${app.ClassName || 'رخصة قيادة'}</td>
                        <td>${nationalID}</td>
                        <td><span style="color: green; font-weight: bold;">${app.Status === 0 ? 'تحت الإجراء' : 'معتمد'}</span></td>
                    </tr>
                `;
            });
        }
    } catch (error) {
        console.error('خطأ في جلب طلبات المستخدم:', error);
    }
}

// الأحداث عند تحميل المستند
document.addEventListener('DOMContentLoaded', () => {
    loadCountries();
    loadLicenseClasses();
    updateHeaderUI();

    // 1. معالجة تسجيل الدخول
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', async function(e) {
            e.preventDefault();
            const loginData = {
                NationalID: document.getElementById('loginNationalID').value.trim(),
                Password: document.getElementById('loginPassword').value.trim()
            };

            try {
                const response = await fetch(`${API_BASE_URL}/people/login`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(loginData)
                });

                if (response.ok) {
                    const result = await response.json();
                    const userData = result.person || result;
                    
                    localStorage.setItem('currentUser', JSON.stringify(userData));
                    updateHeaderUI();
                    alert('تم تسجيل الدخول بنجاح!');
                    loginForm.reset();
                    showPage('home');
                } else {
                    alert('فشل تسجيل الدخول: رقم الهوية أو كلمة السر غير صحيحة.');
                }
            } catch (error) {
                console.error('خطأ في الاتصال:', error);
                alert('حدث خطأ أثناء الاتصال بالخادم.');
            }
        });
    }

    // 2. معالجة إنشاء حساب جديد
    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
        registerForm.addEventListener('submit', async function(e) {
            e.preventDefault();
            const personData = {
                FullName: document.getElementById('regFullName').value,
                PhoneNumber: document.getElementById('regPhone').value,
                Email: document.getElementById('regEmail').value,
                NationalID: document.getElementById('regNationalNo').value,
                Gender: parseInt(document.getElementById('regGender').value),
                CountryID: parseInt(document.getElementById('countrySelect').value),
                City: document.getElementById('regCity').value,
                Password: document.getElementById('regPassword').value
            };

            try {
                const response = await fetch(`${API_BASE_URL}/people`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(personData)
                });

                if (response.ok) {
                    alert('تم إنشاء الحساب وحفظ البيانات بنجاح! يرجى تسجيل الدخول.');
                    registerForm.reset();
                    showPage('login');
                } else {
                    const errorText = await response.text();
                    alert('فشل الحفظ من الخادم: ' + errorText);
                }
            } catch (error) {
                console.error('خطأ في الاتصال:', error);
                alert('حدث خطأ في الاتصال بالـ API.');
            }
        });
    }

    // 3. معالجة طلب رخصة قيادة جديدة وربطها بالجدول OrderDetails (مرتب ومتكامل 100%)
    const newLicenseForm = document.getElementById('newLicenseForm');
    if (newLicenseForm) {
        newLicenseForm.addEventListener('submit', async function(e) {
            e.preventDefault();
            const currentUser = JSON.parse(localStorage.getItem('currentUser'));
            
            if (!currentUser) {
                alert('الرجاء تسجيل الدخول أولاً.');
                showPage('login');
                return;
            }

            // تجهيز البيانات بالكامل متضمنة الحقول الخمسة المطلوبة لتجنب خطأ 500
// معالجة طلب رخصة قيادة جديدة بشكل نظيف ومتكامل
const newLicenseForm = document.getElementById('newLicenseForm');
if (newLicenseForm) {
    newLicenseForm.addEventListener('submit', async function(e) {
        e.preventDefault();
        const currentUser = JSON.parse(localStorage.getItem('currentUser'));
        
        if (!currentUser) {
            alert('الرجاء تسجيل الدخول أولاً.');
            showPage('login');
            return;
        }

        // تجهيز بيانات الطلب متضمنة الحقول الخمسة المطلوبة
        const orderData = {
            PersonID: currentUser.PersonID || currentUser.personID,
            LicenseClassID: parseInt(document.getElementById('licenseClassSelect').value),
            AdminID: currentUser.UserID || currentUser.userID || 1,
            PassedTests: 0,
            Status: 0
        };

        try {
            const response = await fetch(API_BASE_URL + '/orders', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(orderData)
            });

            if (response.ok) {
                const result = await response.json();
                alert(result.message || 'تم تقديم طلب الرخصة الجديدة بنجاح وتسجيله في النظام!');
                newLicenseForm.reset();
                showPage('query'); // الانتقال لصفحة الاستعلام
            } else {
                const errorResult = await response.json();
                alert(errorResult.message || 'فشل حفظ الطلب في الخادم.');
            }
        } catch (error) {
            console.error('خطأ في الاتصال:', error);
            alert('حدث خطأ أثناء إرسال الطلب.');
        }
    });

}
        });
    }
});


// متغير عام لتخزين نتائج البحث الحالية
let currentSearchResults = [];

async function searchLicenses() {
    const nationalID = document.getElementById('queryNationalNo').value.trim();
    const tableBody = document.getElementById('queryTableBody');

    if (!nationalID) {
        alert('الرجاء إدخال رقم الهوية أولاً.');
        return;
    }

    tableBody.innerHTML = `<tr><td colspan="4" style="text-align: center;">جاري البحث...</td></tr>`;

    try {
        let response = await fetch(`http://localhost:5281/api/orders/national/${nationalID}`);

        if (!response.ok) {
            tableBody.innerHTML = `<tr><td colspan="4" style="text-align: center; color: red;">لا توجد رخص أو طلبات مسجلة برقم الهوية هذا.</td></tr>`;
            return;
        }

        let data = await response.json();
        tableBody.innerHTML = ''; 

        currentSearchResults = Array.isArray(data) ? data : [data];

        currentSearchResults.forEach((item) => {
            // اطبع الكائن كاملاً هنا لتراها في الـ Console (اضغط F12 في المتصفح)
            console.log("بيانات الطلب القادمة من الـ API:", item);

            let orderID = item.OrderID || item.orderID || '-';
            
            // محاولة جلب اسم الفئة بجميع الاحتمالات الممكنة
            let licenseClass = item.ClassName || item.className || 
                               item.LicenseClassName || item.licenseClassName || 
                               item.Class_Name || item.License_Class_Name || 'رخصة قيادة';
            
            let status = item.Status !== undefined ? item.Status : (item.status !== undefined ? item.status : 'تحت الإجراء');

            let row = `
                <tr onclick="showOrderDetails(${orderID})" style="cursor: pointer;">
                    <td>${orderID}</td>
                    <td>${licenseClass}</td>
                    <td>${nationalID}</td>
                    <td><span style="color: green; font-weight: bold;">${status}</span></td>
                </tr>
            `;
            tableBody.innerHTML += row;
        });

    } catch (error) {
        console.error('خطأ في الاتصال بالـ API:', error);
        tableBody.innerHTML = `<tr><td colspan="4" style="text-align: center; color: red;">فشل الاتصال بالسيرفر، تأكد من تشغيل الـ API.</td></tr>`;
    }
}

// دالة عرض التفاصيل باستخدام رقم الطلب (OrderID) حصراً
function showOrderDetails(targetOrderID) {
    // البحث عن الطلب المطابق تماماً لرقم الطلب بغض النظر عن ترتيبه
    const item = currentSearchResults.find(o => (o.OrderID == targetOrderID || o.orderID == targetOrderID));
    
    if (!item) {
        // إذا لم يكن موجوداً في نتائج البحث الحالية، نتحقق من الـ localStorage
        const savedOrder = localStorage.getItem('selectedOrder');
        if (savedOrder) {
            const parsed = JSON.parse(savedOrder);
            if (parsed.OrderID == targetOrderID || parsed.orderID == targetOrderID) {
                fillOrderDetails(parsed);
                showPage('details');
                return;
            }
        }
        alert("الرجاء البحث عن الطلب واختياره من الجدول أولاً.");
        return;
    }

    // حفظ الطلب المختار في الذاكرة لكي يظل متاحاً لو دخلت الصفحة مباشرة
    localStorage.setItem('selectedOrder', JSON.stringify(item));
    
    fillOrderDetails(item);
    showPage('details');
}

// دالة مساعدة لتعبئة الحقول بمرونة
function fillOrderDetails(item) {
    const getVal = (obj, keys) => {
        for (let key of keys) {
            for (let objKey in obj) {
                if (objKey.toLowerCase() === key.toLowerCase()) {
                    return obj[objKey];
                }
            }
        }
        return null;
    };

    let orderID = getVal(item, ['OrderID', 'orderID', 'Id', 'id']) || 'غير متوفر';
    let fullName = getVal(item, ['FullName', 'fullName', 'Name', 'name', 'PersonName']) || 'غير متوفرة';
    let status = getVal(item, ['Status', 'status']);
    if (status === null || status === undefined) status = 'تحت الإجراء';

    document.getElementById('detailOrderID').textContent = orderID;
    document.getElementById('detailFullName').textContent = fullName;
    document.getElementById('detailStatus').textContent = status;
}

// تعديل دالة الانتقال بين الصفحات (showPage) لفحص التخزين المحلي عند فتح صفحة التفاصيل
const originalShowPage = window.showPage;
// إذا كانت دالة showPage معرفة لديك مسبقاً، تأكد أنك إذا فتحت صفحة التفاصيل تسترجع آخر طلب مخزن:
function showPage(pageId) {
    // إخفاء كل الصفحات وإظهار المطلوبة (حسب كودك الأساسي)
    document.querySelectorAll('.page').forEach(p => p.classList.remove('active'));
    document.getElementById(pageId).classList.add('active');

    // إذا تم فتح صفحة التفاصيل ولم يتم تعبئتها، نحاول جلب آخر طلب مخزن
    if (pageId === 'details') {
        const savedOrder = localStorage.getItem('selectedOrder');
        if (savedOrder && document.getElementById('detailOrderID').textContent === '-') {
            fillOrderDetails(JSON.parse(savedOrder));
        }
    }
}