<template>
  <nav style="background-color: green; padding: 10px;">
    <h4 class="text-center">IT 06-1</h4>
  </nav>
  <div class="container my-4">

    <!-- Form Add -->
    <div class="container mb-4">
      <div class="input-group" style="max-width: 1200px; margin: auto;">
        <h5 class="text-center" style="padding-inline: 20px;">รหัสสินค้า : </h5>
        <input v-model="newCode" @input="formatInput" @keyup.enter="addCode" placeholder="XXXX-XXXX-XXXX-XXXX"
          class="form-control" maxlength="19" />
        <button @click="addCode" class="btn btn-primary" style="margin-inline: 20px; border-radius: 5px;">
          <!-- <i class="bi bi-plus-lg"></i> --> ADD
        </button>
      </div>
    </div>

    <!-- Table -->
    <table class="table table-hover table-bordered align-middle">
      <thead class="table-primary">
        <tr>
          <th class="text-center">Id</th>
          <th class="text-center">รหัสสินค้า (16 หลัก)</th>
          <th class="text-center">บาร์โค้ดสินค้า</th>
          <!-- <th class="text-center">เพิ่มเมื่อ</th> -->
          <th class="text-center">Action</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(item, index) in codes" :key="item.id">
          <td class="text-center">{{ item.id }}</td>
          <td class="text-center"><code>{{ item.code }}</code></td>
          <td class="text-center">
            <img :src="`/api/productcodes/barcode/${item.code}`" alt="Barcode" height="80" class="border rounded" />
          </td>
          <!-- <td class="text-center">{{ formatDate(item.createdAt) }}</td> -->
          <td class="text-center">
            <button @click="openConfirmDelete(item.id, item.code)" class="btn btn-danger btn-sm">
              <i class="bi bi-trash"></i> ลบ
            </button>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- Modal สำหรับแจ้งเตือน error / success -->
    <div class="modal fade" id="alertModal" tabindex="-1">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="alertModalTitle">{{ alertTitle }}</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          <div class="modal-body">
            {{ alertMessage }}
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-primary" data-bs-dismiss="modal">ตกลง</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal สำหรับยืนยันการลบ -->
    <div class="modal fade" id="confirmModal" tabindex="-1">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header bg-danger text-white">
            <h5 class="modal-title" id="alertModalTitle">{{ alertTitle }}</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          <div class="modal-body">
            ต้องการลบรหัสสินค้า <strong>{{ codeToDelete }}</strong> หรือไม่ ?
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">ยกเลิก</button>
            <button type="button" class="btn btn-danger" @click="confirmDelete">ยืนยัน</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { Modal } from 'bootstrap'

const codes = ref([])
const newCode = ref('')
let codeToDelete = ref('')
let deleteId = ref(null)

const alertTitle = ref('แจ้งเตือน')
const alertMessage = ref('')
const alertType = ref('')

// สร้าง instance modal
let alertModal = null
let confirmModal = null

onMounted(() => {
  loadData()
  // Initialize Bootstrap modals
  alertModal = new Modal(document.getElementById('alertModal'))
  confirmModal = new Modal(document.getElementById('confirmModal'))
})

const showAlert = (title, message, type) => {
  alertTitle.value = title
  alertMessage.value = message
  alertType.value = type

  // เปลี่ยนสีหัวข้อ modal ตาม type
  const header = document.querySelector('#alertModal .modal-header')
  if (header) {
    header.className = 'modal-header'  // ล้างคลาสเก่า
    if (type === 'success') {
      header.classList.add('bg-success', 'text-white')
    } else {
      header.classList.add('bg-danger', 'text-white')
    }
  }

  alertModal.show()
}

const loadData = async () => {
  try {
    const res = await axios.get('/api/productcodes')
    codes.value = res.data
  } catch (err) {
    alert('ไม่สามารถเชื่อมต่อ Server ได้')
    console.error(err)
  }
}

const formatInput = () => {
  let value = newCode.value.replace(/[^a-zA-Z0-9]/g, '').toUpperCase()

  let formatted = ''
  for (let i = 0; i < value.length; i++) {
    if (i > 0 && i % 4 === 0) {
      formatted += '-'
    }
    formatted += value[i]
  }
  newCode.value = formatted
}

const addCode = async () => {
  const displayCode = newCode.value.replace(/-/g, '')
  if (displayCode.length !== 16) {
    showAlert('แจ้งเตือน!','รูปแบบการกรอกรหัสสินค้าไม่ถูกต้อง! ต้องเป็นตัวใหญ่ A-Z หรือ 0-9 เท่านั้น ตัวอย่าง: XXXX-XXXX-XXXX-XXXX', 'error')
    return
  }

  try {
    await axios.post('/api/productcodes', { code: newCode.value })
    newCode.value = ''
    await loadData()
    showAlert('แจ้งเตือน!', `เพิ่มรหัสสินค้าสำเร็จ`, 'success')
  } catch (err) {
    showAlert('แจ้งเตือน!', 'เพิ่มไม่สำเร็จ: ' + (err.response?.data || err.message), 'error')
  }
}

const openConfirmDelete = (id, code) => {
  deleteId.value = id
  codeToDelete.value = code
  confirmModal.show()
}

const confirmDelete = async () => {
  try {
    await axios.delete(`/api/productcodes/${deleteId.value}`)
    confirmModal.hide()
    await loadData()
    showAlert('แจ้งเตือน!', 'ลบรหัสสินค้าเรียบร้อยแล้ว', 'success')
  } catch (err) {
    confirmModal.hide()
    showAlert('แจ้งเตือน!', 'เกิดข้อผิดพลาดในการลบข้อมูล', 'error')
  }
}

// const formatDate = (dateStr) => {
//   return new Date(dateStr).toLocaleString('th-TH')
// }

</script>

<style>
code {
  background: #f8f9fa;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 1.1em;
}
</style>