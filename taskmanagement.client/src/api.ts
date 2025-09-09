import axios from 'axios'

export const api = axios.create({
    baseURL: 'http://localhost:5000/api', // must match backend HTTPS
    headers: { 'Content-Type': 'application/json' }
})