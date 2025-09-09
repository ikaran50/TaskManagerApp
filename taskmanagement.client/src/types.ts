export type TaskItem = {
    id: number
    userId?: number | null
    userName?: string | null
    title: string
    description?: string
    dueDate?: string | null
    isCompleted: boolean
    createdAt: string
    updatedAt: string
}

export type User = {
    id: number
    name: string
    email: string
    isActive: boolean
    createdAt: string
}