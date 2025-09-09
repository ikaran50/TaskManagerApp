import { useState } from 'react'
import { api } from '../api'
import type { User } from '../types'

interface Props {
    onCreatedUser(user: User): void
}

export default function UsersForm({ onCreatedUser }: Props) {
    const [name, setName] = useState('')
    const [email, setEmail] = useState('')
    const [saving, setSaving] = useState(false)

    async function submit(e: React.FormEvent) {
        e.preventDefault()
        if (!name.trim()) return
        setSaving(true)
        try {
            const payload = {
                name,
                email: email || null,
                isActive: true
            }
            const res = await api.post<User>('/users', payload)
            onCreatedUser(res.data)
            setName('');
            setEmail('');
        } finally {
            setSaving(false)
        }
    }

    return (
        <>
            <form onSubmit={submit} className="card" style={{ marginTop: '1rem' }}>
                <div className="row wrap">
                    <input className="input" style={{ flex: 2 }} placeholder="Name" value={name} onChange={e => setName(e.target.value)} />
                    <input className="input" style={{ flex: 3 }} placeholder="Email" value={email} onChange={e => setEmail(e.target.value)} />
                    <button className="btn primary" disabled={saving}>
                        {saving ? 'Adding…' : 'Add User'}
                    </button>
                </div>
            </form>
        </>
    )
}